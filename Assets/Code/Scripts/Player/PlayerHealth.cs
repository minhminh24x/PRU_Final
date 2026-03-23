using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // --- MỚI: Mở public để hàm DebugStats bên kia đọc được ---
    public int currentHP;
    public float currentMP; // Dùng float để MP hồi mượt theo từng khung hình

    [Header("UI Connection")]
    public HealthBarUI healthBar; // Thanh máu (giữ nguyên của bạn)
    public HealthBarUI mpBar;     // Thanh Mana
    public HealthBarUI expBar;    // Thanh EXP

    bool isDead;
    Animator anim;
    Collider2D col;
    PlayerStats stats; // Kết nối với file Stats mới

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        stats = GetComponent<PlayerStats>();

        // Đồng bộ toàn bộ chỉ số gốc từ Stats sang
        if (stats != null)
        {
            stats.Recalculate(); // Đảm bảo tính toán kỹ trước khi gán
            
            if (GameSession.IsLoggedIn && !GameSession.IsNewGame)
            {
                currentHP = GameSession.CurrentHealth;
                // MP hiện tại chưa lưu trên DB, ta có thể set đầy hoặc set mặc định
                currentMP = stats.maxMP; 
            }
            else
            {
                currentHP = stats.maxHP;
                currentMP = stats.maxMP;
            }
            
            UpdateAllUI();
        }
    }

    // --- MỚI: Vòng lặp liên tục để hồi Mana và cập nhật tiến trình EXP ---
    void Update()
    {
        if (isDead || stats == null) return;

        // Logic hồi Mana tự động theo giây
        if (currentMP < stats.maxMP)
        {
            currentMP += stats.manaRegen * Time.deltaTime;
            if (currentMP > stats.maxMP) currentMP = stats.maxMP;
            
            if (mpBar != null) mpBar.SetHealth((int)currentMP);
        }

        // Cập nhật thanh EXP liên tục
        if (expBar != null)
        {
            expBar.SetMaxHealth(stats.GetNextLevelExp());
            expBar.SetHealth(stats.currentExp);
        }
    }

    // --- MỚI: Hàm để các Script Skill gọi vào trừ Mana ---
    public bool UseMana(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            if (mpBar != null) mpBar.SetHealth((int)currentMP);
            return true; // Đủ Mana, cho phép ra chiêu
        }
        Debug.Log("<color=blue>Không đủ Mana!</color>");
        return false; // Thiếu Mana
    }

    // --- SỬA: Thay thế hàm SyncMaxHealth cũ thành LevelUpRestore để hồi đầy cả HP lẫn MP ---
    public void LevelUpRestore()
    {
        currentHP = stats.maxHP;
        currentMP = stats.maxMP;
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        if (healthBar != null) { healthBar.SetMaxHealth(stats.maxHP); healthBar.SetHealth(currentHP); }
        if (mpBar != null) { mpBar.SetMaxHealth(stats.maxMP); mpBar.SetHealth((int)currentMP); }
        if (expBar != null) { expBar.SetMaxHealth(stats.GetNextLevelExp()); expBar.SetHealth(stats.currentExp); }
    }

    // ==========================================================
    // CÁC HÀM CŨ CỦA BẠN (ĐƯỢC GIỮ NGUYÊN HOÀN TOÀN)
    // ==========================================================

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;

        if (healthBar != null)
            healthBar.SetHealth(currentHP);

        if (currentHP <= 0)
            Die();
        else
            Hurt();
    }

    void Hurt()
    {
        if (anim != null)
            anim.SetTrigger("hurt");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
            anim.SetTrigger("death");

        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}