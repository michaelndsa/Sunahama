using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("���ʳt��")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float slowSpeed = 1f; // ��q�k�s�ɨ����t��

    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("��q�t��")]
    public float maxStamina = 100f;
    public float staminaDrainRun = 10f;      // �]�B����
    public float staminaDrainBreath = 15f;   // �������
    public float staminaRegen = 5f;          // �𮧦^�_
    public float Stamina;

    [Header("UI ����")]
    [SerializeField] private Slider staminaBar;

    [Header("���A")]
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
            Debug.Log("���a�}�l����");
        }
        else if (context.canceled)
        {
            isHoldingBreath = false;
            Debug.Log("���a��������");
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

        // ����ɤ��ಾ��
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
            currentSpeed = slowSpeed; // ��q�k�s�u��C��
        }

        rb.velocity = moveInput * currentSpeed;
    }

    private void HandleStamina()
    {
        // �������
        if (isHoldingBreath)
        {
            Stamina -= staminaDrainBreath * Time.fixedDeltaTime;
        }
        // �]�B����
        else if (isRunning && moveInput != Vector2.zero && Stamina > 0f)
        {
            Stamina -= staminaDrainRun * Time.fixedDeltaTime;
        }
        // �^�_
        else
        {
            Stamina += staminaRegen * Time.fixedDeltaTime;
        }

        Stamina = Mathf.Clamp(Stamina, 0f, maxStamina);

        // ��q��0�ɭ���
        if (Stamina <= 0f)
        {
            isHoldingBreath = false;
            Debug.Log("���a��������");
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
