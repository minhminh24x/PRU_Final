using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Singleton quản lý UI hội thoại cho Hub Map.
/// Hỗ trợ: tên NPC, portrait, typewriter effect, callback khi kết thúc, và khóa di chuyển Player.
/// </summary>
public class HubDialogueManager : MonoBehaviour
{
    public static HubDialogueManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Kéo thả GameObject PlayerUI (canvas HUD) vào đây để ẩn khi đang dialogue")]
    public GameObject playerHUDCanvas;
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public Image npcPortrait;
    public TextMeshProUGUI dialogueBodyText;
    public TextMeshProUGUI promptText; // "[E] Tiếp tục"

    [Header("Cài đặt")]
    public float typingSpeed = 0.03f;

    // --- Trạng thái nội bộ ---
    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private Action onDialogueFinished;
    private Animator portraitAnimator; // Lấy tự động từ npcPortrait

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
        dialoguePanel.SetActive(false);
        // Lấy Animator trên Portrait Image (nếu có)
        if (npcPortrait != null)
            portraitAnimator = npcPortrait.GetComponent<Animator>();
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                // Nhấn E khi đang gõ → hiện ngay toàn bộ dòng hiện tại
                StopAllCoroutines();
                dialogueBodyText.text = currentLines[currentIndex];
                isTyping = false;
                if (promptText) promptText.text = "[E] Tiếp tục";
            }
            else
            {
                NextLine();
            }
        }
    }

    /// <summary>
    /// Gọi từ NPCDialogue để mở hội thoại NPC.
    /// </summary>
    public void StartNPCDialogue(NPCData npcData, Action onFinished = null)
    {
        if (npcData == null || npcData.dialogueLines == null || npcData.dialogueLines.Length == 0) return;

        currentLines = npcData.dialogueLines;
        onDialogueFinished = onFinished;
        currentIndex = 0;

        // Gán thông tin NPC lên UI
        if (npcNameText) npcNameText.text = npcData.npcName;

        // Animation portrait: dùng Animator Controller của NPC nếu có
        if (npcPortrait != null)
        {
            if (portraitAnimator != null && npcData.portraitController != null)
            {
                npcPortrait.gameObject.SetActive(true);
                portraitAnimator.runtimeAnimatorController = npcData.portraitController;
                portraitAnimator.SetTrigger("Speak");
            }
            else
            {
                // Fallback: sprite tĩnh
                npcPortrait.gameObject.SetActive(npcData.portrait != null);
                if (npcData.portrait) npcPortrait.sprite = npcData.portrait;
            }
        }

        dialoguePanel.SetActive(true);
        LockPlayer(true);

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    /// <summary>
    /// Gọi từ AncientPedestal để hiện lore (không có portrait NPC).
    /// </summary>
    public void StartPedestalDialogue(string title, string[] lines, Action onFinished = null)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        onDialogueFinished = onFinished;
        currentIndex = 0;

        if (npcNameText) npcNameText.text = title;
        if (npcPortrait) npcPortrait.gameObject.SetActive(false);

        dialoguePanel.SetActive(true);
        LockPlayer(true);

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        currentIndex++;

        if (currentIndex < currentLines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        LockPlayer(false);

        // Reset portrait
        if (portraitAnimator != null)
            portraitAnimator.runtimeAnimatorController = null;
        if (npcPortrait != null)
            npcPortrait.gameObject.SetActive(false);

        // Gọi callback (tặng reward, v.v.)
        onDialogueFinished?.Invoke();
        onDialogueFinished = null;
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueBodyText.text = "";
        if (promptText) promptText.text = "[E] Bỏ qua";

        foreach (char c in currentLines[currentIndex])
        {
            dialogueBodyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        bool isLastLine = (currentIndex >= currentLines.Length - 1);
        if (promptText) promptText.text = isLastLine ? "[E] Đóng" : "[E] Tiếp tục";
    }

    /// <summary>
    /// Khóa / mở khóa di chuyển Player trong lúc đọc hội thoại.
    /// </summary>
    void LockPlayer(bool locked)
    {
        // Tìm PlayerMovement trong scene (dùng Singleton nếu có, hoặc FindObjectOfType)
        var movement = FindFirstObjectByType<PlayerMovement>();
        if (movement != null) movement.enabled = !locked;

        // Ẩn / hiện PlayerUI HUD
        if (playerHUDCanvas != null)
            playerHUDCanvas.SetActive(!locked);
    }
}
