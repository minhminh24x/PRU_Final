using UnityEngine;

public class AggroZone : MonoBehaviour
{
  public BossAI boss;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      boss.SetAggro(true);
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      boss.SetAggro(false);
    }
  }
}
