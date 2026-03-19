using UnityEngine;

[DisallowMultipleComponent]
public class RangedEnemyController : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyRangedCombat rangedCombat;

    [Header("Senses")]
    [SerializeField] DetectSensorRanged2D detectSensor;
    [Tooltip("Dùng AttackRange2D như ShootRange cho ranged.")]
    [SerializeField] AttackRange2D shootRange;

    [Header("Debug")]
    [SerializeField] bool showStateLogs = true;

    enum AIState { Patrol, Chase, Attack }
    AIState _state = AIState.Patrol;

    bool _isInitialized;

    void Start()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (rangedCombat == null) rangedCombat = GetComponent<EnemyRangedCombat>();

        // Khuyên gán tay trong Inspector, có fallback theo tên
        if (detectSensor == null)
            detectSensor = transform.Find("RangedDetect")?.GetComponent<DetectSensorRanged2D>()
                        ?? transform.Find("DetectSensor")?.GetComponent<DetectSensorRanged2D>();

        if (shootRange == null)
            shootRange = transform.Find("ShootRange_")?.GetComponent<AttackRange2D>()
                       ?? transform.Find("ShootRange")?.GetComponent<AttackRange2D>()
                       ?? transform.Find("AttackRange")?.GetComponent<AttackRange2D>();

        if (movement == null) Debug.LogError("[RangedEnemyController] Missing EnemyMovement.", this);
        if (rangedCombat == null) Debug.LogError("[RangedEnemyController] Missing EnemyRangedCombat.", this);
        if (detectSensor == null) Debug.LogError("[RangedEnemyController] Missing DetectSensorRanged2D.", this);
        if (shootRange == null) Debug.LogError("[RangedEnemyController] Missing AttackRange2D (ShootRange).", this);

        _isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!_isInitialized) return;
        Tick();
    }

    void Tick()
    {
        // 1) ATTACK (ShootRange)
        if (shootRange != null && shootRange.InRange && shootRange.Target != null)
        {
            SwitchState(AIState.Attack);

            float targetX = shootRange.Target.position.x;

            if (movement != null)
            {
                // giống close: có gọi MoveTowardX rồi Stop (để face đúng hướng)
                movement.MoveTowardX(targetX);
                movement.Stop();
            }

            if (rangedCombat != null)
                rangedCombat.TryAttack(shootRange.Target);

            return;
        }

        // 2) DETECT & CHASE
        if (detectSensor != null && detectSensor.HasTarget && detectSensor.Target != null)
        {
            SwitchState(AIState.Chase);

            float targetX = detectSensor.Target.position.x;
            if (movement != null) movement.MoveTowardX(targetX);

            return;
        }

        // 3) PATROL
        SwitchState(AIState.Patrol);
        if (movement != null) movement.PatrolTick(Time.fixedDeltaTime);
    }

    void SwitchState(AIState newState)
    {
        if (_state == newState) return;
        _state = newState;

        if (!showStateLogs) return;

        switch (newState)
        {
            case AIState.Attack:
                Debug.Log("<color=red>[AI STATE] ATTACK/SHOOT</color>", this);
                break;
            case AIState.Chase:
                Debug.Log("<color=yellow>[AI STATE] CHASE</color>", this);
                break;
            case AIState.Patrol:
                Debug.Log("<color=cyan>[AI STATE] PATROL</color>", this);
                break;
        }
    }
}
