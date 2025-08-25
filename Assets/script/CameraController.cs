using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("�ؼШ���")]
    public Transform target;

    [Header("�۾����H�t��")]
    public float MoveSmoothTime = 0.2f;  // ���ʮɬ۾����H�����Ʈɶ�

    [Header("����ɬX�M����t��")]
    public float StopSmoothTime = 0.1f;  // ����ɹ�����������Ʈɶ�

    [Header("�۾��Z��")]
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("�������")]
    public float ppu = 32f;

    [Header("�۾����")]
    public bool useBounds = false; // �O�_�ҥά۾����
    public Vector2 minBounds;      // ���U�� (x, y)
    public Vector2 maxBounds;      // �k�W�� (x, y)


    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        
        if (target == null) return;

        Vector3 startPos = target.position + offset;

        // �@�}�l�N�������
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

        // ����ɤ�����u��֪��v����Ҧ�
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null && rb.velocity.magnitude < 0.01f)
        {
            desiredPosition.x = Mathf.Round(desiredPosition.x * ppu) / ppu;
            desiredPosition.y = Mathf.Round(desiredPosition.y * ppu) / ppu;
            smoothTime = StopSmoothTime;
        }

        // �ϥ� SmoothDamp�A�L�ײ��ʩΰ�����ƾa��
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