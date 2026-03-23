using UnityEngine;

[CreateAssetMenu(fileName = "TankBehavior", menuName = "Game/AI/Tank Behavior")]
public class TankAIBehavior : EnemyAIBehavior
{
    [Header("Tank Multipliers")]
    [SerializeField] float speedMultiplier = 0.5f; // Rất chậm
    [SerializeField] float chaseDistMultiplier = 2.0f; // Truy đuổi rất xa

    public override void Execute(EnemyController controller)
    {
        // Tank logic is basically a slow, relentless melee chaser
        float maxDist = controller.MaxChaseDistance * chaseDistMultiplier;
        float pSpeed = controller.PatrolSpeed * speedMultiplier;
        float cSpeed = controller.ChaseSpeed * speedMultiplier;

        // Ensure gravity is on (tanks are usually ground units)
        if (controller.Movement != null) controller.Movement.SetGravityScale(3f); // Heavier

        // 1. ATTACK
        if (controller.AttackRange != null && controller.AttackRange.InRange && controller.AttackRange.Target != null)
        {
            controller.SwitchStateToAttack();
            if (controller.Movement != null) 
            {
                controller.Movement.MoveTowardX(controller.AttackRange.Target.position.x);
                controller.Movement.Stop();
            }
            if (controller.Combat != null) controller.Combat.TryAttack();
            return;
        }

        // 2. DETECT & CHASE
        if (controller.DetectSensor != null && controller.DetectSensor.HasTarget && controller.DetectSensor.Target != null)
        {
            float playerX = controller.DetectSensor.Target.position.x;
            float distPlayerToHome = Mathf.Abs(playerX - controller.HomeX);
            
            // Tank returns home but only if really far
            if (distPlayerToHome > maxDist)
            {
                controller.SwitchStateToReturning();
                if (controller.Movement != null) 
                {
                    controller.Movement.currentSpeed = cSpeed;
                    controller.Movement.MoveTowardX(controller.HomeX);
                }
            }
            else
            {
                controller.SwitchStateToChase();
                if (controller.Movement != null) 
                {
                    controller.Movement.currentSpeed = cSpeed;
                    controller.Movement.MoveTowardX(playerX);
                }
            }
            return;
        }

        // 3. PATROL
        controller.SwitchStateToPatrol();
        if (controller.Movement != null)
        {
            controller.Movement.currentSpeed = pSpeed;
            controller.Movement.PatrolTick(Time.fixedDeltaTime);
        }
    }
}
