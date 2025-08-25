using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    [Header("���w��¦�j�p")]
    public float baseSize = 2f;

    [Header("�I�l���T (��j�Y�p�T��)")]
    public float flickerAmount = 0.2f;

    [Header("�I�l�t��")]
    public float flickerSpeed = 2f;

    [Header("�z���׽d�� (0~1)")]
    [Range(0f, 0.1f)] public float minAlpha = 0.01f;
    [Range(0f, 0.1f)] public float maxAlpha = 0.1f;

    private Vector3 initialScale;
    private SpriteRenderer sr;

    void Start()
    {
        // �O����l�j�p�]�p�G�A�b Inspector ��ʽվ�L���w�j�p�A�N�|�Ψ��ӡ^
        initialScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // �p��I�l�Y�� (�b 1 �� flickerAmount �d�򤺪i��)
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