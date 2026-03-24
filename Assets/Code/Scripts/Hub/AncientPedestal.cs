using UnityEngine;
using TMPro;

/// <summary>
/// Gắn lên Bệ Đá Cổ (Ancient Pedestal) trong Hub Map.
/// - Chưa có ngọc: hiện lore mô tả huyền thoại về boss.
/// - Đã có ngọc:  hiện sprite ngọc + lore xác nhận đã mang ngọc về.
/// Player đến gần + nhấn E → đọc ghi chép.
/// </summary>
public class AncientPedestal : MonoBehaviour
{
    [Header("Dữ liệu ngọc")]
    [Tooltip("ScriptableObject GemData gắn với bệ đá này.")]
    public GemData gemData;

    [Header("Tiêu đề bệ đá")]
    public string pedestalTitle = "Ghi chép cổ";

    [Header("Visual ngọc trên bệ")]
    [Tooltip("SpriteRenderer hiển thị ngọc khi đã đặt (ban đầu ẩn)")]
    public SpriteRenderer gemSlotRenderer;
    [Tooltip("GameObject hiệu ứng glow / particle khi ngọc đã đặt")]
    public GameObject gemGlowEffect;

    [Header("UI Prompt")]
    [Tooltip("TextMeshPro nhỏ phía trên bệ đá, ví dụ: '[E] Đọc'")]
    public TextMeshPro interactPrompt;

    // ─── Trạng thái nội bộ ────────────────────────────────────────────────
    private bool playerNearby = false;
    private bool isReading    = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        // Áp trạng thái ngọc ngay khi load scene
        RefreshGemVisual();
    }

    void Update()
    {
        if (!playerNearby || isReading) return;

        // Không mở lại nếu HubDialogueManager đang hiện panel (tránh E đóng lại mở ngay)
        if (HubDialogueManager.Instance != null &&
            HubDialogueManager.Instance.dialoguePanel != null &&
            HubDialogueManager.Instance.dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.E))
            OpenInscription();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);
    }

    // ─── Public ───────────────────────────────────────────────────────────

    /// <summary>Gọi từ GemManager mỗi khi trạng thái ngọc thay đổi.</summary>
    public void RefreshGemVisual()
    {
        if (gemData == null) return;

        bool collected = GemManager.Instance != null &&
                         GemManager.Instance.IsCollected(gemData.gemType);

        // Cập nhật sprite ngọc trên bệ
        if (gemSlotRenderer != null)
        {
            gemSlotRenderer.gameObject.SetActive(collected);
            if (collected && gemData.gemSprite != null)
                gemSlotRenderer.sprite = gemData.gemSprite;
        }

        // Cập nhật glow effect
        if (gemGlowEffect != null)
            gemGlowEffect.SetActive(collected);
    }

    // ─── Nội bộ ───────────────────────────────────────────────────────────

    void OpenInscription()
    {
        if (gemData == null)
        {
            Debug.LogWarning($"[AncientPedestal] '{pedestalTitle}' chưa gán GemData!");
            return;
        }

        if (HubDialogueManager.Instance == null)
        {
            Debug.LogWarning("[AncientPedestal] Không tìm thấy HubDialogueManager trong scene!");
            return;
        }

        // Chọn lore theo trạng thái ngọc
        bool collected = GemManager.Instance != null &&
                         GemManager.Instance.IsCollected(gemData.gemType);

        string[] loreLines = collected ? gemData.obtainedLore : gemData.pedestalLore;

        if (loreLines == null || loreLines.Length == 0)
        {
            Debug.LogWarning($"[AncientPedestal] '{pedestalTitle}' không có lore phù hợp!");
            return;
        }

        isReading = true;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

        HubDialogueManager.Instance.StartPedestalDialogue(pedestalTitle, loreLines, OnReadingFinished);
    }

    void OnReadingFinished()
    {
        isReading = false;

        if (playerNearby && interactPrompt != null)
            interactPrompt.gameObject.SetActive(true);
    }
}
