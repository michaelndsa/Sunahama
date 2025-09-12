using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CollectMDUI : MonoBehaviour
{
    public Image progressImage; // ��ζi�ױ�
    public TMP_Text keyHintText;  // ���� Z ����

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
            // �N UI ���H�ؼ� MD
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetMD.position + offset);
            transform.position = screenPos;
        }
    }
    private void UpdateKeyHint()
    {
        if (Gamepad.current == null)
        {
            // �S�������� �� ��L
            keyHintText.text = "Z";
            return;
        }

        string layout = Gamepad.current.layout;
        string name = Gamepad.current.displayName;

        if (layout.Contains("XInput") || name.Contains("Xbox"))
        {
            keyHintText.text = "X"; // Xbox �� X ��
        }
        else if (layout.Contains("DualShock") || name.Contains("PlayStation") || name.Contains("Wireless"))
        {
            keyHintText.text = "��"; // PS �������
        }
        else if (layout.Contains("Switch") || name.Contains("Nintendo"))
        {
            keyHintText.text = "Y"; // Switch �� B ��
        }
        else
        {
            keyHintText.text = "?"; // �w�]�������
        }
    }
}
