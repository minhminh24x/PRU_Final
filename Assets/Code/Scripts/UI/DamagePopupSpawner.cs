using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
  public DamagePopup prefab;
  public Transform spawnPoint;

  public void ShowDamage(int dmg, bool crit, int combo = 1)
  {
    if (prefab == null)
    {
      Debug.LogError("❌ DamagePopup prefab chưa được gắn!");
      return;
    }

    Vector3 pos = spawnPoint != null
        ? spawnPoint.position
        : transform.position + Vector3.up;

    DamagePopup popup = Instantiate(prefab, pos, Quaternion.identity);

    popup.Setup(dmg, crit, combo);
  }
}
