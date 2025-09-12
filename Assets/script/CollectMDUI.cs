using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CollectMDUI : MonoBehaviour
{
    public Image progressImage; // 圓形進度條
    public TMP_Text keyHintText;  // 中間 Z 提示

    private Transform targetMD;
    private Vector3 offset;

    public void Show(Transform md, Vector3 customOffset)
    {
        targetMD = md;
        offset = customOffset;
        gameObject.SetActive(true);
        UpdateKeyHint();
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
    private void UpdateKeyHint()
    {
        if (Gamepad.current == null)
        {
            // 沒偵測到手把 → 鍵盤
            keyHintText.text = "Z";
            return;
        }

        string layout = Gamepad.current.layout;
        string name = Gamepad.current.displayName;

        if (layout.Contains("XInput") || name.Contains("Xbox"))
        {
            keyHintText.text = "X"; // Xbox 的 X 鍵
        }
        else if (layout.Contains("DualShock") || name.Contains("PlayStation") || name.Contains("Wireless"))
        {
            keyHintText.text = "□"; // PS 的方塊鍵
        }
        else if (layout.Contains("Switch") || name.Contains("Nintendo"))
        {
            keyHintText.text = "Y"; // Switch 的 B 鍵
        }
        else
        {
            keyHintText.text = "?"; // 預設未知手把
        }
    }
}
