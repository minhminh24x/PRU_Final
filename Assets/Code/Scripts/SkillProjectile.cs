using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
  public float speed = 8f;
  public float lifeTime = 1.2f;

  void Start()
  {
    Destroy(gameObject, lifeTime);
  }

  void Update()
  {
    transform.Translate(Vector2.right * speed * Time.deltaTime);
  }
}
