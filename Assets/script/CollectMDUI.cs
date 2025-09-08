using UnityEngine;
using UnityEngine.UI;

public class CollectMDUI : MonoBehaviour
{
    public Image progressImage; // 圓形進度條
    public GameObject zKeyObj;  // 中間 Z 提示

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
            // 將 UI 跟隨目標 MD
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetMD.position + offset);
            transform.position = screenPos;
        }
    }
}