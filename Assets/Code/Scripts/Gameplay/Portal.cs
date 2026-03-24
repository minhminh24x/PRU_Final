using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;
    public string spawnPointName;
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isTransitioning) return;

        // Tự động chuyển scene khi player bước vào portal
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        isTransitioning = true;
        Debug.Log($"[Portal] Chuyển scene: {sceneToLoad}, SpawnPointName: {spawnPointName}");

        GameSpawnManager.NextSceneName = sceneToLoad;
        GameSpawnManager.SpawnPointName = spawnPointName;
        GameSpawnManager.HasSpawnPosition = true;

        if (fadeImage != null)
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
            yield return null;

        if (fadeImage != null)
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        isTransitioning = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadeImage == null) yield break;
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
