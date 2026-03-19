using UnityEngine;

[DisallowMultipleComponent]
public class DetectSensorRanged2D : MonoBehaviour
{
    [Header("Detect Filter")]
    [SerializeField] string playerTag = "Player";

    [Header("Line of Sight")]
    [Tooltip("Điểm raycast để check LOS. Nếu null sẽ dùng transform hiện tại.")]
    [SerializeField] Transform eyePoint;

    [Tooltip("Layer của tường/vật cản. Raycast hit layer này => mất LOS.")]
    [SerializeField] LayerMask obstacleMask;

    [Tooltip("Check LOS mỗi N giây. Set nhỏ (0.05~0.2).")]
    [SerializeField] float losCheckInterval = 0.1f;

    [Header("Debug")]
    [Tooltip("Bật để in log khi detect / mất target.")]
    [SerializeField] bool debugLogs = true;

    [Tooltip("In trạng thái mỗi N giây (tránh spam). Set 0 = tắt.")]
    [SerializeField] float statusLogInterval = 0.5f;

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;

    public bool HasLineOfSight { get; private set; }

    float _nextLosTime;
    float _nextStatusLogTime;

    void Awake()
    {
        if (eyePoint == null) eyePoint = transform;
    }

    void OnEnable()
    {
        Target = null;
        HasLineOfSight = false;
        _nextLosTime = 0f;
        _nextStatusLogTime = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Target = other.transform;
        ForceLosCheck();

        if (debugLogs)
            Debug.Log($"[DetectRanged] DETECTED: {other.name} (LOS={HasLineOfSight})", this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Rescue: nếu vì lý do nào đó Enter không chạy
        if (Target != null) return;
        if (!other.CompareTag(playerTag)) return;

        Target = other.transform;
        ForceLosCheck();

        if (debugLogs)
            Debug.Log($"[DetectRanged] DETECTED (Stay): {other.name} (LOS={HasLineOfSight})", this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Target != null && other.transform == Target)
        {
            Target = null;
            HasLineOfSight = false;

            if (debugLogs)
                Debug.Log("[DetectRanged] NO PLAYER (Exit)", this);
        }
    }

    void Update()
    {
        // Nếu player bị disable/destroy mà không trigger Exit
        if (Target != null && !Target.gameObject.activeInHierarchy)
        {
            Target = null;
            HasLineOfSight = false;

            if (debugLogs)
                Debug.Log("[DetectRanged] NO PLAYER (Target disabled)", this);
        }

        // Check LOS theo interval
        if (Target != null && Time.time >= _nextLosTime)
        {
            _nextLosTime = Time.time + Mathf.Max(0.01f, losCheckInterval);
            HasLineOfSight = CheckLOS(Target);
        }

        // Debug trạng thái theo interval (không spam mỗi frame)
        if (!debugLogs) return;
        if (statusLogInterval <= 0f) return;

        if (Time.time >= _nextStatusLogTime)
        {
            _nextStatusLogTime = Time.time + statusLogInterval;

            if (HasTarget)
                Debug.Log($"[DetectRanged] STATUS: DETECTED ({Target.name}) LOS={HasLineOfSight}", this);
            else
                Debug.Log("[DetectRanged] STATUS: NO PLAYER", this);
        }
    }

    void ForceLosCheck()
    {
        _nextLosTime = 0f;
        HasLineOfSight = (Target != null) && CheckLOS(Target);
    }

    bool CheckLOS(Transform target)
    {
        if (target == null) return false;

        Vector2 from = (eyePoint != null) ? (Vector2)eyePoint.position : (Vector2)transform.position;
        Vector2 to = (Vector2)target.position;

        Vector2 delta = to - from;
        float dist = delta.magnitude;
        if (dist <= 0.0001f) return true;

        Vector2 dir = delta / dist;

        // Nếu raycast gặp obstacle trước khi tới player => mất LOS
        RaycastHit2D hit = Physics2D.Raycast(from, dir, dist, obstacleMask);
        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (Target == null) return;

        Vector3 from = (eyePoint != null) ? eyePoint.position : transform.position;
        Gizmos.color = HasLineOfSight ? Color.green : Color.red;
        Gizmos.DrawLine(from, Target.position);
    }
}
