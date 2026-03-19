using UnityEngine;

public class SkillSlash : MonoBehaviour
{
  public float lifeTime = 0.4f;

  void Start()
  {
    Destroy(gameObject, lifeTime);
  }
}
