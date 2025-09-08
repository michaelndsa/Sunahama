using UnityEngine;
using UnityEngine.UI;

public class CollectMDUI : MonoBehaviour
{
    public Image progressImage; // ��ζi�ױ�
    public GameObject zKeyObj;  // ���� Z ����

    private Transform targetMD;
    private Vector3 offset;

    public void Show(Transform md, Vector3 customOffset)
    {
        targetMD = md;
        offset = customOffset;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        targetMD = null;
        gameObject.SetActive(false);
    }

    public void SetProgress(float t)
    {
        if (progressImage != null)
            progressImage.fillAmount = Mathf.Clamp01(t);
    }
    private void LateUpdate()
    {
        if (targetMD != null)
        {
            // �N UI ���H�ؼ� MD
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetMD.position + offset);
            transform.position = screenPos;
        }
    }
}