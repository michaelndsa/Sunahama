using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    [Header("光暈基礎大小")]
    public float baseSize = 2f;
    public float AtHoldBreathSize = 0.5f;

    [Header("呼吸振幅 (放大縮小幅度)")]
    public float flickerAmount = 0.2f;

    [Header("呼吸速度")]
    public float normalFlickerSpeed = 2f;
    public float holdBreathFlickerSpeed = 0.5f; // 閉氣時呼吸變慢

    [Header("透明度範圍 (0~1)")]
    [Range(0f, 0.1f)] public float minAlpha = 0.01f;
    [Range(0f, 0.1f)] public float maxAlpha = 0.1f;
    [Range(0f, 1f)] public float minHoldBreathAlpha = 0.8f;
    [Range(0f, 1f)] public float maxHoldBreathAlpha = 1f;

    [Header("閉氣顏色 (Inspector 可調整)")]
    public Color normalColor = Color.white;
    public Color holdBreathColor = Color.black;

    [Header("縮放平滑速度")]
    public float smoothSpeed = 2f;

    [Header("顏色平滑過渡速度")]
    public float colorSmoothSpeed = 3f;

    [Header("蒐集完成後全螢幕大小")]
    public float fullScreenSize = 20f;

    [Header("黑底物件 (Inspector 指派)")]
    public GameObject blackBackground;

    // 成員變數
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
    private bool allCollected = false; // 蒐集完成 flag

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
            // 光暈放大到全螢幕
            usedSize = Mathf.Lerp(usedSize, fullScreenSize, Time.deltaTime * smoothSpeed);
            transform.localScale = initialScale * usedSize;

            if (sr != null)
            {
                Color c = sr.color;

                if (!reachedFullAlpha)
                {
                    // 漸變到白 + alpha = 1
                    c = Color.Lerp(c, Color.white, Time.deltaTime * colorSmoothSpeed);
                    c.a = Mathf.Lerp(c.a, 1f, Time.deltaTime * smoothSpeed);

                    if (c.a >= 0.99f)   // 到頂 → 進入淡出
                    {
                        reachedFullAlpha = true;
                    }
                }
                else
                {
                    // 開始淡出
                    c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * smoothSpeed);

                    if (c.a <= 0.01f)
                    {
                        gameObject.SetActive(false); // 失效
                    }
                }

                sr.color = c;
            }

            // 黑底逐漸消失
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

        // === 一般呼吸/閉氣邏輯 ===
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

    // 當蒐集完全部面玉時呼叫這個
    public void OnAllCollected()
    {
        allCollected = true;
    }
}