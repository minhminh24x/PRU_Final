using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Tiến trình Level")]
    public int level = 1;
    public int currentExp = 0;
    public int unspentStatPoints = 0; 

    [Header("Chỉ số Gốc (Tự tăng theo Level)")]
    public int baseDEF = 100; // Đóng vai trò là Máu gốc
    public int baseINT = 50;  // Đóng vai trò là Mana gốc
    public int baseSTR = 10;
    public int baseAGI = 5;

    [Header("Chỉ số Cộng thêm (Điểm nâng + Đồ đạc)")]
    public int extraDEF = 0;
    public int extraINT = 0;
    public int extraSTR = 0;
    public int extraAGI = 0;

    [Header("Chỉ số Đầu ra (Cho Combat & Health)")]
    public int maxHP;
    public int maxMP;
    public int bonusDamage;
    public float critRate;
    public float attackSpeed;
    public float manaRegen;

    // --- MỚI: Chuyển mảng EXP ra ngoài để các hàm khác có thể đọc được ---
    private readonly int[] expMilestones = { 0, 100, 300, 650, 1150, 1850, 2750, 3950, 5450, 7450 };

    void Start()
    {
        // --- Nạp dữ liệu từ GameSession nếu đã chơi ---
        if (GameSession.IsLoggedIn && !GameSession.IsNewGame)
        {
            level = GameSession.PlayerLevel;
            currentExp = GameSession.CurrentExp;
            unspentStatPoints = GameSession.UnspentStatPoints;
            
            baseDEF = GameSession.BaseDEF;
            baseINT = GameSession.BaseINT;
            baseSTR = GameSession.BaseSTR;
            baseAGI = GameSession.BaseAGI;
            
            extraDEF = GameSession.ExtraDEF;
            extraINT = GameSession.ExtraINT;
            extraSTR = GameSession.ExtraSTR;
            extraAGI = GameSession.ExtraAGI;
        }

        Recalculate();
    }

    void Update()
    {
        // --- MỚI: Bấm P để in toàn bộ chỉ số ra Console test ---
        if (Input.GetKeyDown(KeyCode.P))
        {
            DebugStats();
        }
    }

    public void Recalculate()
    {
        int totalSTR = baseSTR + extraSTR;
        int totalAGI = baseAGI + extraAGI;

        maxHP = baseDEF + (extraDEF * 10);
        maxMP = baseINT + (extraINT * 5);
        bonusDamage = totalSTR * 2;
        critRate = totalAGI * 0.005f;
        attackSpeed = 1f + (totalAGI * 0.01f);

        int totalINT = baseINT + extraINT;
        manaRegen = 1f + (totalINT * 0.2f);
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        
        // --- SỬA LOGIC: Dùng while thay vì if để phòng trường hợp nhận lượng EXP quá lớn lên liền 2, 3 cấp ---
        while (level < expMilestones.Length && currentExp >= expMilestones[level])
        {
            level++;
            unspentStatPoints += 2; 
            baseDEF += 10;
            baseINT += 5;

            Recalculate();
            GetComponent<PlayerHealth>()?.LevelUpRestore(); // Sửa lại gọi hàm đồng bộ mới
            Debug.Log($"<color=cyan>LÊN CẤP {level}! Có {unspentStatPoints} điểm cộng.</color>");
        }
    }

    // --- MỚI: Trả về lượng EXP cần thiết của cấp độ tiếp theo để thanh UI đọc được ---
    public int GetNextLevelExp()
    {
        if (level < expMilestones.Length) return expMilestones[level];
        return expMilestones[expMilestones.Length - 1]; // Trả về mốc cuối nếu max level
    }

    public void DebugStats()
    {
        PlayerHealth hp = GetComponent<PlayerHealth>();
        Debug.Log($"<color=cyan>=== BẢNG CHỈ SỐ PLAYER (Level {level}) ===</color>\n" +
                  $"EXP: {currentExp} / {GetNextLevelExp()} | Điểm dư: {unspentStatPoints}\n" +
                  $"HP: {hp?.currentHP}/{maxHP} | MP: {(int)(hp?.currentMP ?? 0)}/{maxMP} (Hồi {manaRegen}/s)\n" +
                  $"Sát thương cộng thêm: +{bonusDamage} | Tỉ lệ chí mạng: {critRate * 100}% | Tốc đánh: {attackSpeed}\n" +
                  $"STR gốc: {baseSTR} | AGI gốc: {baseAGI} | DEF gốc: {baseDEF} | INT gốc: {baseINT}");
    }
}