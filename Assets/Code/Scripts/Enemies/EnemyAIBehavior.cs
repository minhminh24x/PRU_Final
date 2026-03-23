using UnityEngine;

public abstract class EnemyAIBehavior : ScriptableObject
{
    public abstract void Execute(EnemyController controller);
}
