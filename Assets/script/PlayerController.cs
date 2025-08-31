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

    public bool IsHoldingBreath => isHoldingBreath;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Stamina = maxStamina;
    }

    // ========= InputSystem Callbacks =========
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }

    public void OnHoldBreath(InputAction.CallbackContext context)
    {
        if (context.started && Stamina > 0f)
        {
            isHoldingBreath = true;
            Debug.Log("玩家開始閉氣");
        }
        else if (context.canceled)
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
    }

    private void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        // 閉氣時不能移動
        if (isHoldingBreath && Stamina > 0f)
        {
            currentSpeed = 0f;
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
        // 閉氣消耗
        if (isHoldingBreath)
        {
            Stamina -= staminaDrainBreath * Time.fixedDeltaTime;
        }
        // 跑步消耗
        else if (isRunning && moveInput != Vector2.zero && Stamina > 0f)
        {
            Stamina -= staminaDrainRun * Time.fixedDeltaTime;
        }
        // 回復
        else
        {
            Stamina += staminaRegen * Time.fixedDeltaTime;
        }

        Stamina = Mathf.Clamp(Stamina, 0f, maxStamina);

        // 氣量為0時限制
        if (Stamina <= 0f)
        {
            isHoldingBreath = false;
            Debug.Log("玩家結束閉氣");
            canRun = false;
            isRunning = false;
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
}
