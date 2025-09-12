using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    [Header("���w��¦�j�p")]
    public float baseSize = 2f;
    public float AtHoldBreathSize = 0.5f;

    [Header("�I�l���T (��j�Y�p�T��)")]
    public float flickerAmount = 0.2f;

    [Header("�I�l�t��")]
    public float normalFlickerSpeed = 2f;
    public float holdBreathFlickerSpeed = 0.5f; // ����ɩI�l�ܺC

    [Header("�z���׽d�� (0~1)")]
    [Range(0f, 0.1f)] public float minAlpha = 0.01f;
    [Range(0f, 0.1f)] public float maxAlpha = 0.1f;

    [Header("�����C�� (Inspector �i�վ�)")]
    public Color normalColor = Color.white;
    public Color holdBreathColor = Color.black;

    [Header("�Y�񥭷Ƴt��")]
    public float smoothSpeed = 2f;

    [Header("�C�⥭�ƹL��t��")]
    public float colorSmoothSpeed = 3f;

    // �����ܼ�
    private float usedSize;
    public float targetSize;
    private Vector3 initialScale;
    private SpriteRenderer sr;
    private PlayerController pl;

    private float wave;
    private float scalePulse;
    private float alpha;
    private Color targetColor;
    private Color currentColor;
    private float targetFlickerSpeed;

    void Start()
    {
        initialScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        pl = FindObjectOfType<PlayerController>();
        usedSize = baseSize;
        targetSize = baseSize;

        if (sr != null)
        {
            sr.color = normalColor;
        }
    }

    void Update()
    {
        // �]�w�ؼФj�p
        if (pl != null && pl.IsHoldingBreath)
        {
            targetSize = AtHoldBreathSize;
            targetFlickerSpeed = holdBreathFlickerSpeed;
            targetColor = holdBreathColor;
        }
        else
        {
            targetSize = baseSize;
            targetFlickerSpeed = normalFlickerSpeed;
            targetColor = normalColor;
        }

        // �����Y�񥭷ƹL��
        usedSize = Mathf.Lerp(usedSize, targetSize, Time.deltaTime * smoothSpeed);

        // �I�l�Y��i��
        wave = Mathf.Sin(Time.time * targetFlickerSpeed);
        scalePulse = 1f + flickerAmount * wave;
        transform.localScale = initialScale * usedSize * scalePulse;

        if (sr != null)
        {
            // �I�l�z���תi��
            alpha = Mathf.Lerp(minAlpha, maxAlpha, (wave + 1f) * 0.5f);

            // �C�⥭�ƹL��
            currentColor = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorSmoothSpeed);
            currentColor.a = alpha; // �O�d�z����
            sr.color = currentColor;
        }
    }
}