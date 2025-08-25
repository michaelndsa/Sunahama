using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    [Header("光暈基礎大小")]
    public float baseSize = 2f;

    [Header("呼吸振幅 (放大縮小幅度)")]
    public float flickerAmount = 0.2f;

    [Header("呼吸速度")]
    public float flickerSpeed = 2f;

    [Header("透明度範圍 (0~1)")]
    [Range(0f, 0.1f)] public float minAlpha = 0.01f;
    [Range(0f, 0.1f)] public float maxAlpha = 0.1f;

    private Vector3 initialScale;
    private SpriteRenderer sr;

    void Start()
    {
        // 記錄初始大小（如果你在 Inspector 手動調整過光暈大小，就會用那個）
        initialScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 計算呼吸係數 (在 1 ± flickerAmount 範圍內波動)
        float wave = Mathf.Sin(Time.time * flickerSpeed);

        float scalePulse = 1f + flickerAmount * Mathf.Sin(Time.time * flickerSpeed);
        transform.localScale = initialScale * baseSize * scalePulse;

        if (sr != null)
        {
            float t = (wave + 1f) * 0.5f;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}