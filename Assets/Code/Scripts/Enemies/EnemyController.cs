using UnityEngine;

[DisallowMultipleComponent]
public class EnemyController : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyCombat combat;
    [SerializeField] EnemyHealth health;

    [Header("Senses")]
    [SerializeField] DetectSensor2D detectSensor;
    [SerializeField] AttackRange2D attackRange;

    [Header("Chase Limit (Fallback if no Data)")]
    [Tooltip("Khoảng cách tối đa Enemy được phép rời xa TRUNG TÂM TUẦN TRA")]
    [SerializeField] float fallbackMaxChaseDistance = 15f;
    [SerializeField] float fallbackPatrolSpeed = 2f;
    [SerializeField] float fallbackChaseSpeed = 3.5f;

    [Header("Debug")]
    [SerializeField] bool showStateLogs = true;

    enum AIState { Patrol, Chase, Attack, Returning }
    AIState _currentState = AIState.Patrol;

    float _homeX;
    bool _isInitialized = false;

    void Start()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (combat == null) combat = GetComponent<EnemyCombat>();
        if (health == null) health = GetComponent<EnemyHealth>();
        if (detectSensor == null) detectSensor = transform.Find("DetectSensor")?.GetComponent<DetectSensor2D>();
        if (attackRange == null) attackRange = transform.Find("AttackRange")?.GetComponent<AttackRange2D>();

        if (movement != null)
        {
            _homeX = movement.GetPatrolCenter();
            Debug.Log($"[AI Init] Home X set to: {_homeX}");
        }
        else
        {
            _homeX = transform.position.x;
        }

        _isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!_isInitialized) return;
        HandleAIState();
    }

    void HandleAIState()
    {
        // Get data values
        float maxDist = (health != null && health.data != null) ? health.data.maxChaseDistance : fallbackMaxChaseDistance;
        float pSpeed = (health != null && health.data != null) ? health.data.patrolSpeed : fallbackPatrolSpeed;
        float cSpeed = (health != null && health.data != null) ? health.data.chaseSpeed : fallbackChaseSpeed;

        if (attackRange != null && attackRange.InRange && attackRange.Target != null)
        {
            SwitchState(AIState.Attack);
            if (movement != null) movement.MoveTowardX(attackRange.Target.position.x);
            if (movement != null) movement.Stop();
            if (combat != null) combat.TryAttack();
            return;
        }

        // 2. DETECT & CHASE
        if (detectSensor != null && detectSensor.HasTarget && detectSensor.Target != null)
        {
            float playerX = detectSensor.Target.position.x;
            float distPlayerToHome = Mathf.Abs(playerX - _homeX);
            
            // Check max chase dist
            if (distPlayerToHome > maxDist)
            {
                // Qua gioi han -> Return
                SwitchState(AIState.Returning);
                if (movement != null) 
                {
                    movement.currentSpeed = cSpeed; // ve lẹ
                    movement.MoveTowardX(_homeX);
                }
            }
            else
            {
                SwitchState(AIState.Chase);
                if (movement != null) 
                {
                    movement.currentSpeed = cSpeed;
                    movement.MoveTowardX(playerX);
                }
            }
            return;
        }

        // Returning state logic (if player left sensor)
        if (_currentState == AIState.Returning)
        {
            float distToHome = Mathf.Abs(transform.position.x - _homeX);
            if (distToHome > 0.5f) // chưa về tới
            {
                if (movement != null)
                {
                    movement.currentSpeed = cSpeed;
                    movement.MoveTowardX(_homeX);
                }
                return; // Giữ nguyên Returning
            }
        }

        // 3. PATROL
        SwitchState(AIState.Patrol);
        if (movement != null)
        {
            movement.currentSpeed = pSpeed;
            movement.PatrolTick(Time.fixedDeltaTime);
        }
    }

    void SwitchState(AIState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        if (showStateLogs)
        {
            switch (newState)
            {
                case AIState.Attack:
                    Debug.Log($"<color=red>[AI STATE] ATTACKING!</color>");
                    break;
                case AIState.Chase:
                    Debug.Log($"<color=yellow>[AI STATE] START CHASING (Player detected!)</color>");
                    break;
                case AIState.Patrol:
                    Debug.Log($"<color=cyan>[AI STATE] PATROLLING</color>");
                    break;
                case AIState.Returning:
                    Debug.Log($"<color=grey>[AI STATE] STOP CHASE (Too far from home)</color>");
                    break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        float maxDist = fallbackMaxChaseDistance;
        // if playing and we have health, try get from data
        if (Application.isPlaying && health != null && health.data != null)
        {
            maxDist = health.data.maxChaseDistance;
        }

        Gizmos.color = Color.yellow;
        float center = Application.isPlaying ? _homeX : transform.position.x;
        Vector3 leftLimit = new Vector3(center - maxDist, transform.position.y, 0);
        Vector3 rightLimit = new Vector3(center + maxDist, transform.position.y, 0);

        Gizmos.DrawLine(leftLimit + Vector3.up, leftLimit + Vector3.down);
        Gizmos.DrawLine(rightLimit + Vector3.up, rightLimit + Vector3.down);
        Gizmos.DrawLine(leftLimit, rightLimit);
    }
}