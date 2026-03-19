using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    EnemyCombat _combat;

    void Awake()
    {
        _combat = GetComponentInParent<EnemyCombat>();
    }

    public void DealDamage()
    {
        if (_combat != null)
        {
            _combat.DealDamage();
        }
    }
}
