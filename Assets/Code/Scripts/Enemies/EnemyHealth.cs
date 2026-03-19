using System;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Data (kéo ScriptableObject vào)")]
    public EnemyData data;

    [Header("References")]
    [SerializeField] EnemyAnimDriver animDriver;

    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDied;

    void Awake()
    {
        if (animDriver == null)
            animDriver = GetComponent<EnemyAnimDriver>();
    }

    void Start()
    {
        if (data != null)
            CurrentHP = data.maxHP;
        else
            CurrentHP = 30; // fallback
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHP -= amount;

        if (CurrentHP > 0)
        {
            // Hurt animation
            if (animDriver != null) animDriver.PlayHurt();
            return;
        }

        Die();
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // Animation
        if (animDriver != null) animDriver.PlayDie();

        // Cho EXP
        if (data != null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                    stats.AddExp(data.expReward);
            }
        }

        // Rớt loot
        SpawnLoot();

        // Event để TutorialManager hoặc script khác lắng nghe
        OnDied?.Invoke();

        // Disable components
        var controller = GetComponent<EnemyController>();
        if (controller != null) controller.enabled = false;

        var movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.enabled = false;

        var combat = GetComponent<EnemyCombat>();
        if (combat != null) combat.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Destroy sau 1.5s (chờ animation chết)
        Destroy(gameObject, 1.5f);
    }

    void SpawnLoot()
    {
        if (data == null) return;

        Vector3 pos = transform.position;
        float offsetX = 0f;

        // Gold (luôn rớt)
        if (data.goldPickupPrefab != null)
        {
            int goldAmount = UnityEngine.Random.Range(data.goldMin, data.goldMax + 1);
            var goldObj = Instantiate(data.goldPickupPrefab, pos + Vector3.right * offsetX, Quaternion.identity);
            var goldPickup = goldObj.GetComponent<GoldPickup>();
            if (goldPickup != null)
                goldPickup.amount = goldAmount;
            offsetX += 0.5f;
        }

        // Health Potion (theo tỷ lệ) — prefab đã gắn sẵn ItemData
        if (data.healthPotionPrefab != null && UnityEngine.Random.value <= data.healthPotionChance)
        {
            Instantiate(data.healthPotionPrefab, pos + Vector3.right * offsetX, Quaternion.identity);
            offsetX += 0.5f;
        }

        // Mana Potion (theo tỷ lệ) — prefab đã gắn sẵn ItemData
        if (data.manaPotionPrefab != null && UnityEngine.Random.value <= data.manaPotionChance)
        {
            Instantiate(data.manaPotionPrefab, pos + Vector3.right * offsetX, Quaternion.identity);
        }
    }
}
