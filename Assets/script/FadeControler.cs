using System.Collections;
using UnityEngine;
using UnityEngine.UI;    // �Y�� CanvasGroup �i��� UnityEngine.CanvasGroup

public class ScreenFadeController : MonoBehaviour
{
    [Header("�H�J/�H�X�ؼ�")]
    public Image whiteFade;          // �Y�� CanvasGroup �Ыŧi CanvasGroup
    [Header("���")]
    public float fadeDuration = 0.6f;

    Coroutine currentCo;

    /// �I�s�G�H��¥�
    public void FadeInWhite()
    {
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(FadeRoutine(0, 1));
    }

    /// �I�s�G�q�ղH�^�z��
    public void FadeOutWhite()
    {
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(FadeRoutine(1, 0));
    }

    IEnumerator FadeRoutine(float from, float to)
    {
        float t = 0;
        Color c = whiteFade.color;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            c.a = Mathf.Lerp(from, to, t);
            whiteFade.color = c;
            yield return null;
        }
        // �� alpha �k 0 �ɥi������� Canvas �� Draw Call
        if (to == 0) whiteFade.gameObject.SetActive(false);
    }
}