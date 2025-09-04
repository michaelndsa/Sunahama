using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private bool isHoldingBreath = false;
    private bool isRunning = false;
    private bool canRun = true;

    [Header("圖層調整")]
    [SerializeField]
    private float yOffSet = 0f;

    public bool IsHoldingBreath => isHoldingBreath;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Stamina = maxStamina;
    }

    // ========= 外部設定方法 =========
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetRunning(bool running) => isRunning = running;
    public void SetHoldBreath(bool hold)
    {
        if (hold && Stamina > 0f)
        {
            isHoldingBreath = true;
            Debug.Log("玩家開始閉氣");
        }
        else
        {
            isHoldingBreath = false;
            Debug.Log("玩家結束閉氣");
        }
    }

    // ========= Update =========
    private void FixedUpdate()
    {
        HandleMovement();
        HandleStamina();
        UpdateUI();
        UpdateSortingOrder();
    }

    private void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        if (isHoldingBreath && Stamina > 0f)
        {
            currentSpeed = 0f; // 閉氣時不能移動
        }
        else if (isRunning && canRun && Stamina > 0f)
        {
            currentSpeed = runSpeed;
        }
        else if (!canRun)
        {
            currentSpeed = slowSpeed; // 氣量歸零只能慢走
        }

        rb.velocity = moveInput * currentSpeed;
    }

    private void HandleStamina()
    {
        if (isHoldingBreath)
        {
            Stamina -= staminaDrainBreath * Time.fixedDeltaTime;
        }
        else if (isRunning && moveInput != Vector2.zero && Stamina > 0f)
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
            isHoldingBreath = false;
            canRun = false;
            isRunning = false;
            Debug.Log("玩家結束閉氣 (氣量耗盡)");
        }
        else
        {
            canRun = true;
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
