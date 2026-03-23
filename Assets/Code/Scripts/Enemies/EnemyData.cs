using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Thông tin")]
    public string enemyName = "Skeleton";

    [Header("Stats")]
    public int maxHP = 30;
    public int damage = 5;
    public int expReward = 10;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    [Tooltip("Khoảng cách tối đa Enemy được phép rời xa TRUNG TÂM TUẦN TRA")]
    public float maxChaseDistance = 15f;

    [Header("Gold Drop")]
    public int goldMin = 1;
    public int goldMax = 3;

    [Header("Loot Drop (tỷ lệ 0-1)")]
    [Range(0f, 1f)] public float healthPotionChance = 0.1f;
    [Range(0f, 1f)] public float manaPotionChance = 0.1f;
    public int healthPotionValue = 50;
    public int manaPotionValue = 30;

    [Header("Loot Prefabs (kéo prefab vào)")]
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;
    public GameObject goldPickupPrefab;

    [Header("Combat")]
    public float attackCooldown = 1.0f;
    public float attackRadius = 0.5f;

    [Header("AI Logic")]
    public EnemyAIBehavior aiBehavior;
}
