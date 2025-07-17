using System.Collections;
using UnityEngine;
using UnityEngine.UI;    // 若用 CanvasGroup 可改用 UnityEngine.CanvasGroup

public class ScreenFadeController : MonoBehaviour
{
    [Header("淡入/淡出目標")]
    public Image whiteFade;          // 若用 CanvasGroup 請宣告 CanvasGroup
    [Header("秒數")]
    public float fadeDuration = 0.6f;

    Coroutine currentCo;

    /// 呼叫：淡到純白
    public void FadeInWhite()
    {
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(FadeRoutine(0, 1));
    }

    /// 呼叫：從白淡回透明
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
        // 當 alpha 歸 0 時可選擇關閉 Canvas 省 Draw Call
        if (to == 0) whiteFade.gameObject.SetActive(false);
    }
}