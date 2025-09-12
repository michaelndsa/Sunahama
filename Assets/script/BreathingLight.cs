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
    [Range(0f, 1f)] public float minHoldBreathAlpha = 0.8f;
    [Range(0f, 1f)] public float maxHoldBreathAlpha = 1f;

    [Header("�����C�� (Inspector �i�վ�)")]
    public Color normalColor = Color.white;
    public Color holdBreathColor = Color.black;

    [Header("�Y�񥭷Ƴt��")]
    public float smoothSpeed = 2f;

    [Header("�C�⥭�ƹL��t��")]
    public float colorSmoothSpeed = 3f;

    [Header("�`����������ù��j�p")]
    public float fullScreenSize = 20f;

    [Header("�©����� (Inspector ����)")]
    public GameObject blackBackground;

    // �����ܼ�
    private float usedSize;
    public float targetSize;
    private Vector3 initialScale;
    private SpriteRenderer sr;
    private PlayerController pl;

    private float wave;

    private float minUsedAlpha;
    private float maxUsedAlpha;
    private float scalePulse;
    private float alpha;
    private Color targetColor;
    private Color currentColor;
    private float targetFlickerSpeed;

    private bool reachedFullAlpha = false;
    private bool allCollected = false; // �`������ flag

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
        if (allCollected)
        {
            // ���w��j����ù�
            usedSize = Mathf.Lerp(usedSize, fullScreenSize, Time.deltaTime * smoothSpeed);
            transform.localScale = initialScale * usedSize;

            if (sr != null)
            {
                Color c = sr.color;

                if (!reachedFullAlpha)
                {
                    // ���ܨ�� + alpha = 1
                    c = Color.Lerp(c, Color.white, Time.deltaTime * colorSmoothSpeed);
                    c.a = Mathf.Lerp(c.a, 1f, Time.deltaTime * smoothSpeed);

                    if (c.a >= 0.99f)   // �쳻 �� �i�J�H�X
                    {
                        reachedFullAlpha = true;
                    }
                }
                else
                {
                    // �}�l�H�X
                    c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * smoothSpeed);

                    if (c.a <= 0.01f)
                    {
                        gameObject.SetActive(false); // ����
                    }
                }

                sr.color = c;
            }

            // �©��v������
            if (blackBackground != null)
            {
                SpriteRenderer br = blackBackground.GetComponent<SpriteRenderer>();
                if (br != null)
                {
                    Color bc = br.color;
                    bc.a = Mathf.Lerp(bc.a, 0f, Time.deltaTime * smoothSpeed);
                    br.color = bc;

                    if (bc.a <= 0.01f)
                        blackBackground.SetActive(false);
                }
            }

            return;
        }

        // === �@��I�l/�����޿� ===
        if (pl != null && pl.IsHoldingBreath)
        {
            targetSize = AtHoldBreathSize;
            targetFlickerSpeed = holdBreathFlickerSpeed;
            targetColor = holdBreathColor;
            minUsedAlpha = minHoldBreathAlpha;
            maxUsedAlpha = maxHoldBreathAlpha;
        }
        else
        {
            targetSize = baseSize;
            targetFlickerSpeed = normalFlickerSpeed;
            targetColor = normalColor;
            minUsedAlpha = minAlpha;
            maxUsedAlpha = maxAlpha;
        }

        usedSize = Mathf.Lerp(usedSize, targetSize, Time.deltaTime * smoothSpeed);

        wave = Mathf.Sin(Time.time * targetFlickerSpeed);
        scalePulse = 1f + flickerAmount * wave;
        transform.localScale = initialScale * usedSize * scalePulse;

        if (sr != null)
        {
            alpha = Mathf.Lerp(minUsedAlpha, maxUsedAlpha, (wave + 1f) * 0.5f);
            currentColor = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorSmoothSpeed);
            currentColor.a = alpha;
            sr.color = currentColor;
        }
    }

    // ��`�����������ɮɩI�s�o��
    public void OnAllCollected()
    {
        allCollected = true;
    }
}