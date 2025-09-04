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

    [Header("�ϼh�վ�")]
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

    // ========= �~���]�w��k =========
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetRunning(bool running) => isRunning = running;
    public void SetHoldBreath(bool hold)
    {
        if (hold && Stamina > 0f)
        {
            isHoldingBreath = true;
            Debug.Log("���a�}�l����");
        }
        else
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
        UpdateSortingOrder();
    }

    private void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        if (isHoldingBreath && Stamina > 0f)
        {
            currentSpeed = 0f; // ����ɤ��ಾ��
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
            Debug.Log("���a�������� (��q�Ӻ�)");
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

    // ========= Layer �ƧǨ� y �b =========
    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt((-transform.position.y * 100)+yOffSet);
        }
    }
}
