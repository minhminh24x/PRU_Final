using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

/// <summary>
/// Trigger chuyển vùng với hiệu ứng fade-to-black + text tên khu vực.
/// Đặt ở rìa phải Làng (→ Bệ Đá) và rìa trái Bệ Đá (→ Làng).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZoneTransitionTrigger : MonoBehaviour
{
    [Header("Điểm đến")]
    public Transform destinationSpawn;

    [Header("Tên khu vực đích")]
    [Tooltip("Text hiện lên giữa màn đen, ví dụ: 'Bệ Đá Cổ' hoặc 'Làng'")]
    public string locationName = "Bệ Đá Cổ";

    [Header("Cinemachine Camera Bounds")]
    public CinemachineConfiner2D cinemachineConfiner;
    public Collider2D newBoundsCollider;
    
    [Tooltip("CinemachineCamera chính để gọi lệnh Snap mượt mà (tránh kéo camera)")]
    public CinemachineCamera cinemachineCamera;

    [Header("Thời gian fade")]
    public float fadeDuration = 0.8f;
    public float holdDuration = 0.6f;

    // ── Shared fallback canvas (tạo 1 lần cho cả scene) ─────────────────────
    private static CanvasGroup _sharedFadeGroup;
    private static TextMeshProUGUI _sharedLocationText;

    private bool isTransitioning = false;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        EnsureFadeCanvas();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (!other.CompareTag("Player")) return;
        if (destinationSpawn == null)
        {
            Debug.LogWarning($"[ZoneTransitionTrigger] '{name}' chưa gán destinationSpawn!");
            return;
        }
        StartCoroutine(DoTransition(other.gameObject));
    }

    IEnumerator DoTransition(GameObject player)
    {
        isTransitioning = true;

        // 1. Khoá Player
        var movement = player.GetComponent<PlayerMovement>();
        var rb       = player.GetComponent<Rigidbody2D>();
        if (movement != null) movement.enabled = false;
        if (rb != null)       rb.linearVelocity = Vector2.zero;

        // 2. Tối dần
        yield return StartCoroutine(LerpAlpha(0f, 1f));

        // 3. Hiện tên khu vực
        SetLocationText(locationName);

        // 4. Giữ màn đen
        yield return new WaitForSeconds(holdDuration);

        // 5. Teleport (lưu vị trí cũ để tính delta)
        Vector3 oldPos = player.transform.position;
        player.transform.position = destinationSpawn.position;
        Vector3 deltaPos = player.transform.position - oldPos;

        // 6. Đổi Camera Bounds
        if (cinemachineConfiner != null && newBoundsCollider != null)
        {
            cinemachineConfiner.BoundingShape2D = newBoundsCollider;
            cinemachineConfiner.InvalidateBoundingShapeCache();
        }

        // 7. Yêu cầu Cinemachine dịch tức thời (Snap) theo Player và đảm bảo đang Target Player
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Target.TrackingTarget = player.transform;
            cinemachineCamera.OnTargetObjectWarped(player.transform, deltaPos);
        }

        // Chờ 1 frame trước khi sáng màn để Unity update Physics/Camera
        yield return null;

        // 8. Xoá text
        SetLocationText("");

        // 9. Sáng dần
        yield return StartCoroutine(LerpAlpha(1f, 0f));

        // 9. Mở khoá Player
        if (movement != null) movement.enabled = true;

        isTransitioning = false;
    }

    // ── Fade helpers ─────────────────────────────────────────────────────────

    IEnumerator LerpAlpha(float from, float to)
    {
        var cg = GetFadeGroup();
        if (cg == null) yield break;

        cg.alpha = from;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = to;
    }

    CanvasGroup GetFadeGroup()
    {
        // Ưu tiên SceneTransitionManager
        if (SceneTransitionManager.Instance != null &&
            SceneTransitionManager.Instance.fadeCanvasGroup != null)
            return SceneTransitionManager.Instance.fadeCanvasGroup;

        return _sharedFadeGroup;
    }

    void SetLocationText(string text)
    {
        // Ưu tiên splashText của SceneTransitionManager
        if (SceneTransitionManager.Instance != null &&
            SceneTransitionManager.Instance.splashText != null)
        {
            SceneTransitionManager.Instance.splashText.text = text;
            return;
        }
        if (_sharedLocationText != null)
            _sharedLocationText.text = text;
    }

    // ── Tạo fallback Canvas (chỉ 1 lần duy nhất cho toàn scene) ─────────────

    void EnsureFadeCanvas()
    {
        if (_sharedFadeGroup != null) return; // đã tạo rồi

        var canvasGO = new GameObject("[ZoneFadeCanvas]");
        DontDestroyOnLoad(canvasGO);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Image đen phủ toàn màn
        var imgGO = new GameObject("BlackImage");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = Color.black;
        var rect = img.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Text tên khu vực (giữa màn)
        var textGO = new GameObject("LocationText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.fontSize = 36;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color  = new Color(1f, 0.9f, 0.6f);
        var trect = tmp.rectTransform;
        trect.anchorMin = new Vector2(0, 0.45f);
        trect.anchorMax = new Vector2(1, 0.55f);
        trect.offsetMin = Vector2.zero;
        trect.offsetMax = Vector2.zero;

        _sharedLocationText = tmp;

        // CanvasGroup để fade
        var cg = canvasGO.AddComponent<CanvasGroup>();
        cg.alpha          = 0f;
        cg.interactable   = false;
        cg.blocksRaycasts = false;

        _sharedFadeGroup = cg;
    }
}
