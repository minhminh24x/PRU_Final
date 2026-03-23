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

    public enum AIState { Patrol, Chase, Attack, Returning }
    AIState _currentState = AIState.Patrol;

    float _homeX;
    bool _isInitialized = false;

    // Public properties for AI Behaviors to access
    public float HomeX => _homeX;
    public EnemyMovement Movement => movement;
    public EnemyCombat Combat => combat;
    public EnemyHealth Health => health;
    public DetectSensor2D DetectSensor => detectSensor;
    public AttackRange2D AttackRange => attackRange;
    public bool IsReturning => _currentState == AIState.Returning;

    public float MaxChaseDistance => (health != null && health.data != null) ? health.data.maxChaseDistance : fallbackMaxChaseDistance;
    public float PatrolSpeed => (health != null && health.data != null) ? health.data.patrolSpeed : fallbackPatrolSpeed;
    public float ChaseSpeed => (health != null && health.data != null) ? health.data.chaseSpeed : fallbackChaseSpeed;

    void Start()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (combat == null) combat = GetComponent<EnemyCombat>();
        if (health == null) health = GetComponent<EnemyHealth>();
        
        // Try to find sensors if not assigned
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
        if (health != null && health.data != null && health.data.aiBehavior != null)
        {
            health.data.aiBehavior.Execute(this);
        }
        else
        {
            // Fallback to legacy logic if no behavior assigned
            DefaultMeleeLogic();
        }
    }

    private void DefaultMeleeLogic()
    {
        float maxDist = MaxChaseDistance;
        float pSpeed = PatrolSpeed;
        float cSpeed = ChaseSpeed;

        if (attackRange != null && attackRange.InRange && attackRange.Target != null)
        {
            SwitchState(AIState.Attack);
            if (movement != null) 
            {
                movement.MoveTowardX(attackRange.Target.position.x);
                movement.Stop();
            }
            if (combat != null) combat.TryAttack();
            return;
        }

        if (detectSensor != null && detectSensor.HasTarget && detectSensor.Target != null)
        {
            float playerX = detectSensor.Target.position.x;
            float distPlayerToHome = Mathf.Abs(playerX - _homeX);
            
            if (distPlayerToHome > maxDist)
            {
                SwitchState(AIState.Returning);
                if (movement != null) 
                {
                    movement.currentSpeed = cSpeed;
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

        if (_currentState == AIState.Returning)
        {
            float distToHome = Mathf.Abs(transform.position.x - _homeX);
            if (distToHome > 0.5f)
            {
                if (movement != null)
                {
                    movement.currentSpeed = cSpeed;
                    movement.MoveTowardX(_homeX);
                }
                return;
            }
        }

        SwitchState(AIState.Patrol);
        if (movement != null)
        {
            movement.currentSpeed = pSpeed;
            movement.PatrolTick(Time.fixedDeltaTime);
        }
    }

    public void SwitchStateToAttack() => SwitchState(AIState.Attack);
    public void SwitchStateToChase() => SwitchState(AIState.Chase);
    public void SwitchStateToPatrol() => SwitchState(AIState.Patrol);
    public void SwitchStateToReturning() => SwitchState(AIState.Returning);

    public void SwitchState(AIState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        if (showStateLogs)
        {
            switch (newState)
            {
                case AIState.Attack:
                    Debug.Log($"<color=red>[AI STATE] ATTACKING!</color>", this);
                    break;
                case AIState.Chase:
                    Debug.Log($"<color=yellow>[AI STATE] START CHASING (Player detected!)</color>", this);
                    break;
                case AIState.Patrol:
                    Debug.Log($"<color=cyan>[AI STATE] PATROLLING</color>", this);
                    break;
                case AIState.Returning:
                    Debug.Log($"<color=grey>[AI STATE] STOP CHASE (Too far from home)</color>", this);
                    break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        float maxDist = MaxChaseDistance;

        Gizmos.color = Color.yellow;
        float center = Application.isPlaying ? _homeX : transform.position.x;
        Vector3 leftLimit = new Vector3(center - maxDist, transform.position.y, 0);
        Vector3 rightLimit = new Vector3(center + maxDist, transform.position.y, 0);

        Gizmos.DrawLine(leftLimit + Vector3.up, leftLimit + Vector3.down);
        Gizmos.DrawLine(rightLimit + Vector3.up, rightLimit + Vector3.down);
        Gizmos.DrawLine(leftLimit, rightLimit);
    }
}