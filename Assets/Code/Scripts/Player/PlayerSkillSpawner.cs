using UnityEngine;

public class PlayerSkillSpawner : MonoBehaviour
{
  public Transform firePoint;

  public GameObject fireSkill;
  public GameObject crescentCross;
  public GameObject flameCrescent;

  PlayerCombat combat;

  void Awake()
  {
    combat = GetComponent<PlayerCombat>();
  }

  // GỌI BẰNG ANIMATION EVENT
  public void SpawnSkill()
  {
    switch (combat.currentAttackMode)
    {
      case AttackMode.FireSkill:
        Instantiate(fireSkill, firePoint.position, firePoint.rotation);
        break;

      case AttackMode.CrescentCross:
        Instantiate(crescentCross, firePoint.position, firePoint.rotation);
        break;

      case AttackMode.FlameCrescent:
        Instantiate(flameCrescent, firePoint.position, firePoint.rotation);
        break;

      case AttackMode.Normal:
      default:
        break;
    }
  }

  public void ResetAttackMode()
  {
    combat.currentAttackMode = AttackMode.Normal;
  }
}
