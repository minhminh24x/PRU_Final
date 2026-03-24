using System.Collections;
using UnityEngine;

/// <summary>
/// Controller riêng cho Boss Rannoch (Earth Titan).
/// Điều phối Phase, các kỹ năng tấn công và Animator.
/// </summary>
public class EarthBossController : MonoBehaviour
{
    [Header("Stats & Phase")]
    public float phase2Threshold = 0.5f; // 50% HP
    public float phase2SpeedMultiplier = 1.2f;
    private bool _isPhase2 = false;

    [Header("Skills Settings")]
    public float attackInterval = 3f;
    public float smashTelegraphTime = 1.5f;
    public GameObject rockPrefab;
    public Transform firePoint;
    public GameObject shockwavePrefab;

    [Header("References")]
    private EnemyController _controller;
    private EnemyHealth _health;
    private Animator _anim;
    private bool _isActionInProgress = false;
    private float _nextActionTime = 0f;

    void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _health = GetComponent<EnemyHealth>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Boss mặc định có Super Armor
        if (_health != null) _health.superArmor = true;
        _nextActionTime = Time.time + 2f; // Chờ 2s trước khi bắt đầu
    }

    void Update()
    {
        if (_health.IsDead) return;

        CheckPhase();

        if (!_isActionInProgress && Time.time >= _nextActionTime)
        {
            ChooseAction();
        }
    }

    void CheckPhase()
    {
        if (!_isPhase2 && _health.CurrentHP <= _health.data.maxHP * phase2Threshold)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        _isPhase2 = true;
        Debug.Log("<color=red>[Boss] ENRAGED! Entering Phase 2</color>");
        
        // Hiệu ứng phát sáng
        if (_anim) _anim.SetTrigger("glow");
        
        // Tăng tốc độ chase (nếu có dùng movement)
        if (_controller.Movement != null)
            _controller.Movement.currentSpeed *= phase2SpeedMultiplier;
            
        // Giảm thời gian chờ giữa các chiêu thức
        attackInterval *= 0.8f; 
    }

    void ChooseAction()
    {
        if (_controller.DetectSensor == null || !_controller.DetectSensor.HasTarget) return;

        // Đơn giản hóa: Luôn ném đá
        StartCoroutine(SkillRockThrow());
    }

    IEnumerator SkillRockThrow()
    {
        _isActionInProgress = true;
        _controller.SwitchStateToAttack();
        _controller.Movement?.Stop();

        Debug.Log("[Boss] Rock Throw!");
        if (_anim) _anim.SetTrigger("shoot");

        yield return new WaitForSeconds(0.5f); // Chờ frame ném

        if (rockPrefab && firePoint)
        {
            GameObject rock = Instantiate(rockPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rrb = rock.GetComponent<Rigidbody2D>();
            if (rrb != null)
            {
                // Tìm hướng Player
                Vector2 dir = (_controller.DetectSensor.Target.position - firePoint.position).normalized;
                // Ném tảng đá lăn lê mặt đất
                rrb.linearVelocity = new Vector2(dir.x * 10f, 0); 
            }
        }

        yield return new WaitForSeconds(1.0f);
        FinishAction();
    }

    void FinishAction()
    {
        _isActionInProgress = false;
        _nextActionTime = Time.time + attackInterval;
        _controller.SwitchStateToChase();
    }
}
