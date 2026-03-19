using UnityEngine;
using TMPro;

/// <summary>
/// Gắn lên GameObject NPC trong Hub Map.
/// - Lần đầu gặp: chạy dialogueLines đầy đủ + tặng reward.
/// - Lần sau (đã nhận reward): chạy repeatDialogueLines ngắn, không reward.
/// - Trạng thái được lưu bằng PlayerPrefs → nhớ ngay cả khi restart game.
/// </summary>
public class NPCDialogue : MonoBehaviour
{
    [Header("Dữ liệu NPC")]
    public NPCData npcData;

    [Header("UI Prompt")]
    [Tooltip("TextMeshPro nhỏ phía trên NPC, ví dụ: '[E] Nói chuyện'")]
    public TextMeshPro interactPrompt;

    // --- Trạng thái runtime ---
    private bool playerNearby = false;
    private bool isDialogueActive = false;
    private bool hasMetBefore; // Đọc từ PlayerPrefs

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        // Đọc trạng thái đã gặp trước đó từ PlayerPrefs
        if (npcData != null)
            hasMetBefore = PlayerPrefs.GetInt(npcData.GetSaveKey(), 0) == 1;
    }

    void Update()
    {
        if (!playerNearby || isDialogueActive) return;
        if (Input.GetKeyDown(KeyCode.E))
            OpenDialogue();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);
    }

    void OpenDialogue()
    {
        if (npcData == null) return;
        if (HubDialogueManager.Instance == null)
        {
            Debug.LogWarning("[NPCDialogue] Không tìm thấy HubDialogueManager trong scene!");
            return;
        }

        isDialogueActive = true;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

        // --- Phân nhánh: lần đầu hay lần sau ---
        if (!hasMetBefore)
        {
            // Lần đầu: hội thoại đầy đủ + reward
            HubDialogueManager.Instance.StartNPCDialogue(npcData, OnFirstMeetingFinished);
        }
        else
        {
            // Lần sau: hội thoại ngắn, không reward
            string[] repeatLines = (npcData.repeatDialogueLines != null && npcData.repeatDialogueLines.Length > 0)
                ? npcData.repeatDialogueLines
                : null;

            if (repeatLines != null)
                HubDialogueManager.Instance.StartPedestalDialogue(npcData.npcName, repeatLines, OnRepeatFinished);
            else
                OnRepeatFinished(); // Không có gì để nói → đóng ngay
        }
    }


    // ─── Callback: lần đầu ──────────────────────────────────────────────────
    void OnFirstMeetingFinished()
    {
        GiveReward();

        // Đánh dấu đã gặp, lưu vĩnh viễn
        hasMetBefore = true;
        PlayerPrefs.SetInt(npcData.GetSaveKey(), 1);
        PlayerPrefs.Save();

        // Delay 1 frame: tránh phím E đóng-dialogue bắt luôn để mở lại
        StartCoroutine(ResetDialogueNextFrame());
    }

    // ─── Callback: lần sau ──────────────────────────────────────────────────
    void OnRepeatFinished()
    {
        // Delay 1 frame: tránh phím E đóng-dialogue bắt luôn để mở lại
        StartCoroutine(ResetDialogueNextFrame());
    }

    // ─── Chờ 1 frame rồi mới reset ─────────────────────────────────────────
    System.Collections.IEnumerator ResetDialogueNextFrame()
    {
        yield return null; // chờ sang frame tiếp theo
        isDialogueActive = false;
        ShowPromptIfNearby();
    }

    void ShowPromptIfNearby()
    {
        if (playerNearby && interactPrompt != null)
            interactPrompt.gameObject.SetActive(true);
    }

    // ─── Tặng reward ────────────────────────────────────────────────────────
    void GiveReward()
    {
        if (npcData == null) return;

        // Tặng item
        if (npcData.rewardItem != null && npcData.rewardAmount > 0 && InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(npcData.rewardItem, npcData.rewardAmount);
            if (added)
                Debug.Log($"<color=yellow>[Hub] {npcData.npcName} tặng {npcData.rewardAmount}x {npcData.rewardItem.itemName}</color>");
        }

        // Hồi Mana trực tiếp
        if (npcData.rewardManaDirect > 0)
        {
            var playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                var stats = playerHealth.GetComponent<PlayerStats>();
                float maxMP = stats != null ? stats.maxMP : 999f;
                playerHealth.currentMP = Mathf.Min(playerHealth.currentMP + npcData.rewardManaDirect, maxMP);
                Debug.Log($"<color=cyan>[Hub] {npcData.npcName} hồi {npcData.rewardManaDirect} Mana</color>");
            }
        }
    }
}
