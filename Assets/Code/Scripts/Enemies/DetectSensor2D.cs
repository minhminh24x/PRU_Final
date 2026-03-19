using UnityEngine;

[DisallowMultipleComponent]
public class DetectSensor2D : MonoBehaviour
{
    [Header("Detect Filter")]
    [SerializeField] string playerTag = "Player";

    [Header("Debug")]
    [SerializeField] bool debugLogs = true;
    [SerializeField] float statusLogInterval = 0.5f;

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;

    float _nextStatusLogTime;

    void OnEnable()
    {
        Target = null;
        _nextStatusLogTime = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Target = other.transform;

        if (debugLogs)
            Debug.Log($"[DetectSensor] DETECTED: {other.name}", this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Target != null) return;
        if (!other.CompareTag(playerTag)) return;

        Target = other.transform;

        if (debugLogs)
            Debug.Log($"[DetectSensor] DETECTED (Stay): {other.name}", this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Target != null && other.transform == Target)
        {
            Target = null;

            if (debugLogs)
                Debug.Log("[DetectSensor] NO PLAYER (Exit)", this);
        }
    }

    void Update()
    {
        if (!debugLogs) return;
        if (statusLogInterval <= 0f) return;

        if (Time.time >= _nextStatusLogTime)
        {
            _nextStatusLogTime = Time.time + statusLogInterval;

            if (HasTarget)
                Debug.Log($"[DetectSensor] STATUS: DETECTED ({Target.name})", this);
            else
                Debug.Log("[DetectSensor] STATUS: NO PLAYER", this);
        }

        if (Target != null && !Target.gameObject.activeInHierarchy)
        {
            Target = null;
            if (debugLogs)
                Debug.Log("[DetectSensor] NO PLAYER (Target disabled)", this);
        }
    }
}
