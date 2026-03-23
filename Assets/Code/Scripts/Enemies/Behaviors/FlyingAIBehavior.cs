using UnityEngine;

[CreateAssetMenu(fileName = "FlyingBehavior", menuName = "Game/AI/Flying Behavior")]
public class FlyingAIBehavior : EnemyAIBehavior
{
    [SerializeField] float hoverHeight = 3.5f;
    [SerializeField] float sineFrequency = 2f;
    [SerializeField] float sineAmplitude = 0.5f;

    public override void Execute(EnemyController controller)
    {
        if (controller.Movement == null) return;

        // Disable gravity while flying
        controller.Movement.SetGravityScale(0f);

        float pSpeed = controller.PatrolSpeed;
        float cSpeed = controller.ChaseSpeed;

        // 1. ATTACK (Dive or shoot)
        if (controller.AttackRange != null && controller.AttackRange.InRange && controller.AttackRange.Target != null)
        {
            controller.SwitchStateToAttack();
            // Flying units dive toward the target
            controller.Movement.MoveToward(controller.AttackRange.Target.position);
            
            if (controller.Combat != null) controller.Combat.TryAttack();
            return;
        }

        // 2. DETECT & CHASE (Stay above player)
        if (controller.DetectSensor != null && controller.DetectSensor.HasTarget && controller.DetectSensor.Target != null)
        {
            controller.SwitchStateToChase();
            controller.Movement.currentSpeed = cSpeed;

            Vector2 targetPos = controller.DetectSensor.Target.position;
            targetPos.y += hoverHeight + Mathf.Sin(Time.time * sineFrequency) * sineAmplitude;

            controller.Movement.MoveToward(targetPos);
            return;
        }

        // 3. PATROL (Move between A and B in the air)
        controller.SwitchStateToPatrol();
        controller.Movement.currentSpeed = pSpeed;
        
        // Custom patrol for flying: simple horizontal patrol but staying at hover height
        // (Just reuse PatrolTick but reset gravity)
        controller.Movement.PatrolTick(Time.fixedDeltaTime);
    }
}
