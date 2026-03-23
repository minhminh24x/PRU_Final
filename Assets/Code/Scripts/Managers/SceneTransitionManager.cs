using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("UI References")]
    public CanvasGroup fadeCanvasGroup;
    public TextMeshProUGUI splashText;

    [Header("Settings")]
    public float fadeDuration = 1f;
    public float textDisplayDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Khi scene vừa load xong, bắt đầu làm sáng màn hình ra
        FadeOut();
    }

    private void Start()
    {
        // Đảm bảo bắt đầu với màn hình đen hoàn toàn nếu chưa có gì
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;
        FadeOut();
    }

    public void EnterSceneWithTransition(string sceneName, string text)
    {
        StartCoroutine(TransitionRoutine(sceneName, text));
    }

    private IEnumerator TransitionRoutine(string sceneName, string text)
    {
        // 1. Tối dần màn hình và hiện text
        if (splashText != null) splashText.text = text;
        yield return StartCoroutine(Fade(1f));

        // 2. Chờ một lát cho người chơi đọc text
        yield return new WaitForSeconds(textDisplayDuration);

        // 3. Load Scene mới
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 4. Sáng dần màn hình ra ở Scene mới
        yield return StartCoroutine(Fade(0f));
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, onComplete));
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, () => {
            if (splashText != null) splashText.text = ""; // Xóa text khi đã sáng màn hình
            onComplete?.Invoke();
        }));
    }

    private IEnumerator Fade(float targetAlpha, System.Action onComplete = null)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}
