using UnityEngine;

[CreateAssetMenu(fileName = "RangedBehavior", menuName = "Game/AI/Ranged Behavior")]
public class RangedAIBehavior : EnemyAIBehavior
{
    public override void Execute(EnemyController controller)
    {
        float pSpeed = controller.PatrolSpeed;
        float cSpeed = controller.ChaseSpeed;

        // 1) ATTACK (ShootRange)
        if (controller.AttackRange != null && controller.AttackRange.InRange && controller.AttackRange.Target != null)
        {
            controller.SwitchStateToAttack();

            float targetX = controller.AttackRange.Target.position.x;

            if (controller.Movement != null)
            {
                controller.Movement.MoveTowardX(targetX);
                controller.Movement.Stop();
            }

            // Note: Ranged behavior expects EnemyRangedCombat
            // We can either try to get it here or have a generic interface
            var rangedCombat = controller.GetComponent<EnemyRangedCombat>();
            if (rangedCombat != null)
                rangedCombat.TryAttack(controller.AttackRange.Target);

            return;
        }

        // 2) DETECT & CHASE
        if (controller.DetectSensor != null && controller.DetectSensor.HasTarget && controller.DetectSensor.Target != null)
        {
            controller.SwitchStateToChase();

            float targetX = controller.DetectSensor.Target.position.x;
            if (controller.Movement != null) 
            {
                controller.Movement.currentSpeed = cSpeed;
                controller.Movement.MoveTowardX(targetX);
            }
            return;
        }

        // 3) PATROL
        controller.SwitchStateToPatrol();
        if (controller.Movement != null) 
        {
            controller.Movement.currentSpeed = pSpeed;
            controller.Movement.PatrolTick(Time.fixedDeltaTime);
        }
    }
}
