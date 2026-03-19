using UnityEngine;
using TMPro;

/// <summary>
/// Gắn lên Bệ Đá Cổ (Ancient Pedestal) trong Hub Map.
/// Player đến gần + nhấn E → hiện lore/ghi chép. Có thể đọc lại không giới hạn.
/// </summary>
public class AncientPedestal : MonoBehaviour
{
    [Header("Nội dung Bệ Đá")]
    public string pedestalTitle = "Ghi chép cổ";

    [TextArea(2, 5)]
    public string[] inscriptionLines;

    [Header("UI Prompt")]
    [Tooltip("TextMeshPro nhỏ phía trên bệ đá, ví dụ: '[E] Đọc'")]
    public TextMeshPro interactPrompt;

    // --- Trạng thái ---
    private bool playerNearby = false;
    private bool isReading = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby || isReading) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenInscription();
        }
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

    void OpenInscription()
    {
        if (inscriptionLines == null || inscriptionLines.Length == 0)
        {
            Debug.LogWarning($"[AncientPedestal] Bệ đá '{pedestalTitle}' chưa có nội dung!");
            return;
        }

        if (HubDialogueManager.Instance == null)
        {
            Debug.LogWarning("[AncientPedestal] Không tìm thấy HubDialogueManager trong scene!");
            return;
        }

        isReading = true;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

        HubDialogueManager.Instance.StartPedestalDialogue(pedestalTitle, inscriptionLines, OnReadingFinished);
    }

    void OnReadingFinished()
    {
        isReading = false;

        // Bệ đá luôn có thể đọc lại → hiện lại prompt
        if (playerNearby && interactPrompt != null)
            interactPrompt.gameObject.SetActive(true);
    }
}
