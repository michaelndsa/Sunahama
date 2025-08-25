using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("目標角色")]
    public Transform target;

    [Header("相機跟隨速度")]
    public float MoveSmoothTime = 0.2f;  // 移動時相機跟隨的平滑時間

    [Header("停止時柔和對齊速度")]
    public float StopSmoothTime = 0.1f;  // 停止時對齊像素的平滑時間

    [Header("相機距離")]
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("像素對齊")]
    public float ppu = 32f;

    [Header("相機邊界")]
    public bool useBounds = false; // 是否啟用相機邊界
    public Vector2 minBounds;      // 左下角 (x, y)
    public Vector2 maxBounds;      // 右上角 (x, y)


    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        
        if (target == null) return;

        Vector3 startPos = target.position + offset;

        // 一開始就像素對齊
        startPos.x = Mathf.Round(startPos.x * ppu) / ppu;
        startPos.y = Mathf.Round(startPos.y * ppu) / ppu;
        startPos.z = offset.z;

        transform.position = startPos;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float smoothTime = MoveSmoothTime;

        // 停止時切換到「更快的」對齊模式
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null && rb.velocity.magnitude < 0.01f)
        {
            desiredPosition.x = Mathf.Round(desiredPosition.x * ppu) / ppu;
            desiredPosition.y = Mathf.Round(desiredPosition.y * ppu) / ppu;
            smoothTime = StopSmoothTime;
        }

        // 使用 SmoothDamp，無論移動或停止都平滑靠近
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        if (useBounds)
        {
            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            float minX = minBounds.x + camWidth / 2f;
            float maxX = maxBounds.x - camWidth / 2f;
            float minY = minBounds.y + camHeight / 2f;
            float maxY = maxBounds.y - camHeight / 2f;

            smoothPosition.x = Mathf.Clamp(smoothPosition.x, minX, maxX);
            smoothPosition.y = Mathf.Clamp(smoothPosition.y, minY, maxY);
        }

        transform.position = smoothPosition;
    }
}