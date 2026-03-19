using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    
    [Header("Patrol")]
    [SerializeField] float arriveDistance = 0.15f;
    [SerializeField] float waitTime = 0.5f;

    [Header("Movement Speed (Set by Controller)")]
    public float currentSpeed = 2f;

    [Header("Chase/MoveTowardX")]
    [Tooltip("Deadzone để không bị flip hướng khi tới rất gần targetX.")]
    [SerializeField] float moveTowardDeadzone = 0.12f;

    [SerializeField] Rigidbody2D rb;

    Transform _target;
    float _waitTimer;
    bool _isWaiting;

    public bool IsWaiting => _isWaiting;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        _target = pointA != null ? pointA : pointB;
    }

    public float GetPatrolCenter()
    {
        if (pointA == null || pointB == null)
            return transform.position.x;

        return (pointA.position.x + pointB.position.x) / 2f;
    }

    public void PatrolTick(float dt)
    {
        if (rb == null) return;
        if (_target == null) return;

        if (_isWaiting)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            _waitTimer += dt;

            if (_waitTimer >= waitTime)
            {
                _waitTimer = 0f;
                _isWaiting = false;

                // đổi điểm
                if (pointA != null && pointB != null)
                    _target = (_target == pointA) ? pointB : pointA;
            }
            return;
        }

        float dx = _target.position.x - rb.position.x;

        // ✅ tới nơi -> STOP NGAY để không overshoot
        if (Mathf.Abs(dx) <= arriveDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            _isWaiting = true;
            _waitTimer = 0f;
            return;
        }

        float dir = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dir * currentSpeed, rb.linearVelocity.y);
        FaceDirection(dir);
    }

    public void MoveTowardX(float targetX)
    {
        if (rb == null) return;

        float dx = targetX - rb.position.x;

        // ✅ Deadzone: quá gần thì dừng, tránh flip -1/+1 liên tục
        if (Mathf.Abs(dx) <= moveTowardDeadzone)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dir = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dir * currentSpeed, rb.linearVelocity.y);
        FaceDirection(dir);
    }

    public void Stop()
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void FaceDirection(float dir)
    {
        if (dir == 0) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir);
        transform.localScale = scale;
    }
}
