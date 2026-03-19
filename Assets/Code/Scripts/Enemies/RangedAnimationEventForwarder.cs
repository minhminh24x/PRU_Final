using UnityEngine;

[DisallowMultipleComponent]
public class RangedAnimationEventForwarder : MonoBehaviour
{
    EnemyRangedCombat _combat;

    void Awake()
    {
        // Lấy EnemyRangedCombat ở parent (Root: RangedSkeleton)
        _combat = GetComponentInParent<EnemyRangedCombat>();

        if (_combat == null)
        {
            Debug.LogError(
                "[RangedAnimationEventForwarder] EnemyRangedCombat not found in parent!",
                this
            );
        }
    }

    /// <summary>
    /// Gọi từ Animation Event tại frame bắn tên.
    /// </summary>
    public void Shoot()
    {
        if (_combat != null)
        {
            _combat.Shoot();
        }
    }
}
