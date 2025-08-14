using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeController : MonoBehaviour
{
    public static SceneFadeController Instance { get; private set; }

    public Image FadeImage;
    [SerializeField] float fadeSpeed = 1f;
    
    enum FadeState { None, FadingIn, FadingOut, WaitToLoad }
    private FadeState currentState = FadeState.None;
    private string nextScene = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (FadeImage == null)
            FadeImage = GetComponentInChildren<Image>();
       
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 切場景後直接淡入
        currentState = FadeState.FadingIn;
        SetAlpha(1f);
    }

    private void Start()
    {
        SetAlpha(0f); // 初始透明
        currentState = FadeState.None;
    }

    private void Update()
    {
        switch (currentState)
        {
            case FadeState.FadingIn:
                FadeIn();
                break;
            case FadeState.FadingOut:
                FadeOut();
                break;
            case FadeState.WaitToLoad:
                SceneManager.LoadScene(nextScene);
                currentState = FadeState.None;
                break;
        }
    }

    // UI 或程式呼叫，開始淡出並切換場景
    public void StartFade(string sceneName)
    {
        nextScene = sceneName;
        currentState = FadeState.FadingOut;
    }

    void FadeIn()
    {
        Color c = FadeImage.color;
        c.a -= fadeSpeed * Time.deltaTime;
        if (c.a <= 0)
        {
            c.a = 0;
            currentState = FadeState.None;
        }
        FadeImage.color = c;
    }

    void FadeOut()
    {
        Color c = FadeImage.color;
        c.a += fadeSpeed * Time.deltaTime;
        if (c.a >= 1)
        {
            c.a = 1;
            currentState = FadeState.WaitToLoad;
        }
        FadeImage.color = c;
    }

    void SetAlpha(float value)
    {
        Color c = FadeImage.color;
        c.a = value;
        FadeImage.color = c;
    }

}