using UnityEngine;

/// <summary>
/// ScriptableObject chứa toàn bộ dữ liệu của một NPC trong Hub Map.
/// Right-click Project → Create > Game/NPC Data để tạo mới.
/// </summary>
[CreateAssetMenu(fileName = "New NPC", menuName = "Game/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("Thông tin NPC")]
    public string npcName = "NPC";
    public Sprite portrait;

    [Header("Portrait Animation")]
    [Tooltip("Animator Controller chứa animation Idle + Speak cho PortraitImage trong dialogue UI.\nĐể trống = dùng portrait tĩnh (Sprite).")]
    public RuntimeAnimatorController portraitController;

    [Header("Hội thoại lần đầu (có reward)")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Hội thoại lần sau (sau khi đã nhận reward)")]
    [Tooltip("Nội dung nói khi Player quay lại gặp NPC ở lần sau. Để trống = NPC không nói gì thêm.")]
    [TextArea(2, 4)]
    public string[] repeatDialogueLines;

    [Header("Phần thưởng (Reward)")]
    [Tooltip("Item tặng sau khi nói chuyện lần đầu. Để trống nếu không tặng item.")]
    public ItemData rewardItem;
    [Tooltip("Số lượng item tặng.")]
    public int rewardAmount = 1;

    [Tooltip("Hồi thêm Mana trực tiếp (không qua Item). 0 = không hồi.")]
    public int rewardManaDirect = 0;

    /// <summary>Key dùng để lưu PlayerPrefs, tự sinh từ tên NPC.</summary>
    public string GetSaveKey() => $"NPC_Met_{npcName.Replace(" ", "_")}";
}
