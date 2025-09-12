using UnityEngine;
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
    public bool IsHoldingBreath = false;
    public bool UsedSpecialAbility = false;
    public bool IsRunning = false;
    public bool CanRun = true;
    public bool IsCollecting = false;
    public bool CanUseAbility = false;

    [Header("�ϼh�վ�")]
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

    // ========= �~���]�w��k =========
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetRunning(bool running) => IsRunning = running;
    public void SetHoldBreath(bool hold)
    {
        if (hold && Stamina > 0f)
        {
            IsHoldingBreath = true;
            Debug.Log("���a�}�l����");
        }
        else
        {
            IsHoldingBreath = false;
            Debug.Log("���a��������");
        }
    }
    public void SetStartCollect(bool hold)
    {
        if (hold)
        {
            IsCollecting = true;
            Debug.Log("���a�}�l�`��");
        }
        else 
        { 
            IsCollecting = false;
            Debug.Log("���a���`��");
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
            // �u���@�j �� ���ĤHŪ��N�|����
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
            currentSpeed = 0f; // ����ɤ��ಾ��
        }
        else if (IsRunning && CanRun && Stamina > 0f)
        {
            currentSpeed = runSpeed;
        }
        else if (!CanRun)
        {
            currentSpeed = slowSpeed; // ��q�k�s�u��C��
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
            Debug.Log("���a�������� (��q�Ӻ�)");
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

    // ========= Layer �ƧǨ� y �b =========
    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt((-transform.position.y * 100)+yOffSet);
        }
    }
}
