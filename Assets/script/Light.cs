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

    [Header("閉氣顏色 (Inspector 可調整)")]
    public Color normalColor = Color.white;
    public Color holdBreathColor = Color.black;

    [Header("縮放平滑速度")]
    public float smoothSpeed = 2f;

    [Header("顏色平滑過渡速度")]
    public float colorSmoothSpeed = 3f;

    // 成員變數
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
        // 設定目標大小
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

        // 光圈縮放平滑過渡
        usedSize = Mathf.Lerp(usedSize, targetSize, Time.deltaTime * smoothSpeed);

        // 呼吸縮放波動
        wave = Mathf.Sin(Time.time * targetFlickerSpeed);
        scalePulse = 1f + flickerAmount * wave;
        transform.localScale = initialScale * usedSize * scalePulse;

        if (sr != null)
        {
            // 呼吸透明度波動
            alpha = Mathf.Lerp(minAlpha, maxAlpha, (wave + 1f) * 0.5f);

            // 顏色平滑過渡
            currentColor = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorSmoothSpeed);
            currentColor.a = alpha; // 保留透明度
            sr.color = currentColor;
        }
    }
}