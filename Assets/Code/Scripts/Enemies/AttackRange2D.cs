using UnityEngine;

[DisallowMultipleComponent]
public class AttackRange2D : MonoBehaviour
{
    [Header("Attack Filter")]
    [SerializeField] string playerTag = "Player";

    [Header("Debug")]
    [SerializeField] bool debugLogs = true;
    [SerializeField] float statusLogInterval = 0.5f;

    public Transform Target { get; private set; }

    // ✅ InRange giờ dựa trên trạng thái + target hợp lệ
    public bool InRange => _inRange && IsTargetValid(Target);

    float _nextStatusLogTime;
    bool _inRange;

    void OnEnable()
    {
        ClearTarget();
        _nextStatusLogTime = 0f;
    }

    void OnDisable()
    {
        ClearTarget();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        SetTarget(other.transform, "Enter");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // ✅ Nếu target hiện tại invalid (bị destroy/disable) thì cho phép refresh lại
        if (!IsTargetValid(Target))
        {
            SetTarget(other.transform, "Stay-Refresh");
            return;
        }

        // ✅ Nếu chưa có target thì set
        if (Target == null)
        {
            SetTarget(other.transform, "Stay");
            return;
        }

        // ✅ Nếu đang có target khác (hiếm) thì ưu tiên target gần hơn hoặc giữ nguyên.
        // (Ở đây giữ nguyên cho đơn giản)
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Target != null && other.transform == Target)
        {
            ClearTarget("Exit");
        }
    }

    void Update()
    {
        // ✅ luôn validate target mỗi frame (fix case Destroy/Disable mà không có Exit)
        if (!IsTargetValid(Target))
        {
            if (Target != null && debugLogs)
                Debug.Log("[AttackRange] OUT OF RANGE (Target invalid)", this);

            ClearTarget();
        }

        // Debug status
        if (!debugLogs) return;
        if (statusLogInterval <= 0f) return;

        if (Time.time >= _nextStatusLogTime)
        {
            _nextStatusLogTime = Time.time + statusLogInterval;

            if (InRange && Target != null)
                Debug.Log($"[AttackRange] STATUS: IN RANGE ({Target.name})", this);
            else
                Debug.Log("[AttackRange] STATUS: OUT OF RANGE", this);
        }
    }

    // -------------------------
    // Helpers
    // -------------------------

    void SetTarget(Transform t, string reason)
    {
        Target = t;
        _inRange = (t != null);

        if (debugLogs && t != null)
            Debug.Log($"[AttackRange] IN RANGE ({reason}): {t.name}", this);
    }

    void ClearTarget(string reason = null)
    {
        Target = null;
        _inRange = false;

        if (debugLogs && !string.IsNullOrEmpty(reason))
            Debug.Log($"[AttackRange] OUT OF RANGE ({reason})", this);
    }

    static bool IsTargetValid(Transform t)
    {
        // Unity destroyed object => (t == null) sẽ true theo operator overload
        if (t == null) return false;

        // bị disable / inactive
        if (!t.gameObject.activeInHierarchy) return false;

        return true;
    }
}
