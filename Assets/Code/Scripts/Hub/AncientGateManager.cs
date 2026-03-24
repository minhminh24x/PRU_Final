using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Singleton quản lý cổng cổ đại ở trung tâm Hub.
/// Được gọi bởi GemManager khi đủ 4 viên ngọc.
/// </summary>
public class AncientGateManager : MonoBehaviour
{
    public static AncientGateManager Instance { get; private set; }

    [Header("Visuals")]
    [Tooltip("Visual cổng khi đóng (deactivated khi mở cổng)")]
    public GameObject gateClosedVisual;
    [Tooltip("Visual cổng khi mở (activated khi đủ ngọc)")]
    public GameObject gateOpenedVisual;

    [Header("Collider chặn đường")]
    [Tooltip("Collider bình thường chặn Player — sẽ bị tắt khi mở cổng")]
    public Collider2D gateBlockCollider;

    [Header("Hint Text")]
    [Tooltip("TextMeshPro hiển thị gợi ý 'Thu thập đủ 4 ngọc...' — ẩn khi cổng mở")]
    public TextMeshPro gateHintText;

    [Header("Hiệu ứng")]
    [Tooltip("Animator trên cổng (optional, trigger 'Open')")]
    public Animator gateAnimator;
    [Tooltip("Particle hiệu ứng mở cổng (optional)")]
    public ParticleSystem openEffect;
    [Tooltip("Thời gian delay trước khi swap visual (giây)")]
    public float openDelay = 1.2f;

    [Header("Trạng thái")]
    [SerializeField] private bool isGateOpen = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Khi scene load: nếu đã đủ ngọc từ lần trước → mở ngay
        if (GemManager.Instance != null && GemManager.Instance.CollectedCount() >= 4)
            OpenGateImmediate();
        else
            CloseGateImmediate();
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>Gọi từ GemManager khi đủ 4 ngọc — có animation.</summary>
    public void ActivateGate()
    {
        if (isGateOpen) return;
        StartCoroutine(OpenGateSequence());
    }

    // ─── Internal ─────────────────────────────────────────────────────────────

    IEnumerator OpenGateSequence()
    {
        isGateOpen = true;

        // 1. Trigger animation (nếu có)
        if (gateAnimator != null)
            gateAnimator.SetTrigger("Open");

        // 2. Particle effect
        if (openEffect != null)
            openEffect.Play();

        // 3. Chờ delay rồi swap visual
        yield return new WaitForSeconds(openDelay);

        SwapVisuals();
    }

    void OpenGateImmediate()
    {
        isGateOpen = true;
        SwapVisuals();
    }

    void CloseGateImmediate()
    {
        isGateOpen = false;
        if (gateClosedVisual != null) gateClosedVisual.SetActive(true);
        if (gateOpenedVisual != null) gateOpenedVisual.SetActive(false);
        if (gateBlockCollider != null) gateBlockCollider.enabled = true;
    }

    void SwapVisuals()
    {
        if (gateClosedVisual != null) gateClosedVisual.SetActive(false);
        if (gateOpenedVisual != null) gateOpenedVisual.SetActive(true);
        if (gateBlockCollider != null) gateBlockCollider.enabled = false;

        // Ẩn gợi ý khi cổng đã mở
        if (gateHintText != null) gateHintText.gameObject.SetActive(false);

        Debug.Log("[AncientGateManager] Cổng cổ đại đã mở!");
    }
}
