using UnityEngine;

[CreateAssetMenu(fileName = "MeleeBehavior", menuName = "Game/AI/Melee Behavior")]
public class MeleeAIBehavior : EnemyAIBehavior
{
    public override void Execute(EnemyController controller)
    {
        // Get data values
        float maxDist = controller.MaxChaseDistance;
        float pSpeed = controller.PatrolSpeed;
        float cSpeed = controller.ChaseSpeed;

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

        // 3. RETURNING
        if (controller.IsReturning)
        {
            float distToHome = Mathf.Abs(controller.transform.position.x - controller.HomeX);
            if (distToHome > 0.5f)
            {
                if (controller.Movement != null)
                {
                    controller.Movement.currentSpeed = cSpeed;
                    controller.Movement.MoveTowardX(controller.HomeX);
                }
                return;
            }
        }

        // 4. PATROL
        controller.SwitchStateToPatrol();
        if (controller.Movement != null)
        {
            controller.Movement.currentSpeed = pSpeed;
            controller.Movement.PatrolTick(Time.fixedDeltaTime);
        }
    }
}
