using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float slowSpeed = 1f; // 氣量歸零時走路速度

    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("氣量系統")]
    public float maxStamina = 100f;
    public float staminaDrainRun = 10f;      // 跑步消耗
    public float staminaDrainBreath = 15f;   // 閉氣消耗
    public float staminaRegen = 5f;          // 休息回復
    public float Stamina;

    [Header("UI 元件")]
    [SerializeField] private Slider staminaBar;

    [Header("狀態")]
    public bool IsHoldingBreath = false;
    public bool UsedSpecialAbility = false;
    public bool IsRunning = false;
    public bool CanRun = true;
    public bool IsCollecting = false;
    public bool CanUseAbility = false;

    [Header("圖層調整")]
    [SerializeField]
    private float yOffSet = 0f;

    private SpriteRenderer spriteRenderer;
    private MendamaCollectUI collectUI;
    private MendamaSpawner spawner;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collectUI = FindObjectOfType<MendamaCollectUI>();
        spawner = FindObjectOfType<MendamaSpawner>();
        Stamina = maxStamina;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleStamina();
        UpdateUI();
        UpdateSortingOrder();
    }

    // ========= 外部設定方法 =========
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetRunning(bool running) => IsRunning = running;
    public void SetHoldBreath(bool hold)
    {
        if (hold && Stamina > 0f)
        {
            IsHoldingBreath = true;
            Debug.Log("玩家開始閉氣");
        }
        else
        {
            IsHoldingBreath = false;
            Debug.Log("玩家結束閉氣");
        }
    }
    public void SetStartCollect(bool hold)
    {
        if (hold)
        {
            IsCollecting = true;
            Debug.Log("玩家開始蒐集");
        }
        else 
        { 
            IsCollecting = false;
            Debug.Log("玩家放棄蒐集");
        }
    }
    public void UseHideAbility()
    {
        if (!CanUseAbility)
            { return; }
        {
            
        }
        if (collectUI.collectCount > 0 )
        {
            // 只打一槍 → 讓敵人讀到就會反應
            UsedSpecialAbility = true;

            collectUI.UpdateUsed();

            spawner.SpawnSingleMendamaFromList();
        }
        else
        {
            return;
        }
    }
    private void HandleMovement()
    {
        if (UsedSpecialAbility)
        {
            rb.velocity = Vector2.zero;
            return;
        }
     
        float currentSpeed = walkSpeed;

        if (IsHoldingBreath && Stamina > 0f)
        {
            currentSpeed = 0f; // 閉氣時不能移動
        }
        else if (IsRunning && CanRun && Stamina > 0f)
        {
            currentSpeed = runSpeed;
        }
        else if (!CanRun)
        {
            currentSpeed = slowSpeed; // 氣量歸零只能慢走
        }

            rb.velocity = moveInput * currentSpeed;
    }

    private void HandleStamina()
    {
        if (IsHoldingBreath)
        {
            Stamina -= staminaDrainBreath * Time.fixedDeltaTime;
        }
        else if (IsRunning && moveInput != Vector2.zero && Stamina > 0f)
        {
            Stamina -= staminaDrainRun * Time.fixedDeltaTime;
        }
        else
        {
            Stamina += staminaRegen * Time.fixedDeltaTime;
        }

        Stamina = Mathf.Clamp(Stamina, 0f, maxStamina);

        if (Stamina <= 0f)
        {
            IsHoldingBreath = false;
            CanRun = false;
            IsRunning = false;
            Debug.Log("玩家結束閉氣 (氣量耗盡)");
        }
        else
        {
            CanRun = true;
        }
    }

    private void UpdateUI()
    {
        if (staminaBar != null)
            staminaBar.value = Stamina;
    }

    // ========= Layer 排序依 y 軸 =========
    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt((-transform.position.y * 100)+yOffSet);
        }
    }
}
