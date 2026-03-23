using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Tự động tạo ra 2 mốc tuần tra từ vị trí ban đầu mà không cần lập Point A, B")]
    [SerializeField] bool autoPatrol = true;
    [SerializeField] float autoPatrolDistance = 3f;

    [Tooltip("Gán tay Point A, B (Bỏ chọn Auto Patrol mới chạy)")]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    
    [Header("Patrol Tuning")]
    [SerializeField] float arriveDistance = 0.15f;
    [SerializeField] float waitTime = 1.0f;

    [Header("Movement Speed (Set by Controller)")]
    public float currentSpeed = 2f;

    [Header("Chase/MoveTowardX")]
    [Tooltip("Deadzone để không bị flip hướng khi tới rất gần targetX.")]
    [SerializeField] float moveTowardDeadzone = 0.12f;

    [SerializeField] Rigidbody2D rb;

    float _pointAX;
    float _pointBX;
    float _targetX;
    float _waitTimer;
    bool _isWaiting;

    public bool IsWaiting => _isWaiting;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Khởi tạo vị trí tuần tra
        if (autoPatrol)
        {
            _pointAX = transform.position.x - autoPatrolDistance;
            _pointBX = transform.position.x + autoPatrolDistance;
        }
        else if (pointA != null && pointB != null)
        {
            _pointAX = pointA.position.x;
            _pointBX = pointB.position.x;
        }
        else
        {
            _pointAX = transform.position.x;
            _pointBX = transform.position.x;
        }

        _targetX = _pointBX; // Bắt đầu đi về phải
    }

    public float GetPatrolCenter()
    {
        return (_pointAX + _pointBX) / 2f;
    }

    public void PatrolTick(float dt)
    {
        if (rb == null) return;
        if (_pointAX == _pointBX) return; // Không có quảng đường tuần tra

        if (_isWaiting)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            _waitTimer += dt;

            if (_waitTimer >= waitTime)
            {
                _waitTimer = 0f;
                _isWaiting = false;

                // đổi điểm
                _targetX = (_targetX == _pointAX) ? _pointBX : _pointAX;
            }
            return;
        }

        float dx = _targetX - rb.position.x;

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

    /// <summary>
    /// Di chuyển theo cả 2 trục (dùng cho quái bay).
    /// </summary>
    public void MoveToward(Vector2 targetPos)
    {
        if (rb == null) return;

        Vector2 currentPos = rb.position;
        Vector2 dir = (targetPos - currentPos);
        
        if (dir.magnitude <= moveTowardDeadzone)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        dir.Normalize();
        rb.linearVelocity = dir * currentSpeed;
        FaceDirection(dir.x);
    }

    public void SetGravityScale(float scale)
    {
        if (rb != null) rb.gravityScale = scale;
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
