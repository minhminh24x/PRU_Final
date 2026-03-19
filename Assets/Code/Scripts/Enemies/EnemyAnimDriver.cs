using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAnimDriver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement movement;

    [Header("Tuning")]
    [SerializeField] float speedDeadzone = 0.2f;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyMovement>();
        anim = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    public void PlayAttack()
    {
        if (anim == null) return;
        anim.SetTrigger("Attack");
    }

    public void PlayHurt()
    {
        if (anim == null) return;
        anim.SetTrigger("Hurt");
    }

    public void PlayDie()
    {
        if (anim == null) return;
        anim.SetTrigger("Die");
    }

    void Update()
    {
        if (anim == null) return;

        float speed;

        if (movement != null && movement.IsWaiting)
        {
            speed = 0f;
        }
        else
        {
            speed = Mathf.Abs(rb != null ? rb.linearVelocity.x : 0f);
            if (speed < speedDeadzone) speed = 0f;
        }

        anim.SetFloat("Speed", speed);
    }
}
