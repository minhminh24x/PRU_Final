using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
  [Header("Move")]
  public float moveUpSpeed = 1.5f;
  public float sideMoveRange = 0.6f;
  public float curveStrength = 3f;

  [Header("Life")]
  public float lifeTime = 0.8f;

  [Header("Crit Effect")]
  public float critScaleMultiplier = 1.3f;
  public float critShakeIntensity = 0.15f;
  public float critShakeDuration = 0.15f;

  TextMeshProUGUI text;

  Vector3 startPos;
  float xDirection;
  float timeAlive;
  float shakeTimer;

  void Awake()
  {
    text = GetComponentInChildren<TextMeshProUGUI>();

    startPos = transform.position;
    xDirection = Random.Range(-sideMoveRange, sideMoveRange);

    Destroy(gameObject, lifeTime);
  }

  // Damage thường
  public void Setup(int damage, bool crit)
  {
    Setup(damage, crit, 1);
  }

  // Damage combo
  public void Setup(int damage, bool crit, int combo)
  {
    text.text = combo > 1
        ? $"{damage}  x{combo}"
        : damage.ToString();

    text.color = crit ? Color.yellow : Color.white;

    if (crit)
    {
      text.fontSize *= critScaleMultiplier;
      shakeTimer = critShakeDuration;
    }
  }

  void Update()
  {
    timeAlive += Time.deltaTime;

    // 🔹 Bay cong parabol
    float curve = Mathf.Sin(timeAlive * curveStrength);

    Vector3 offset = new Vector3(
        xDirection * curve,
        moveUpSpeed * timeAlive,
        0
    );

    // 🔥 LUÔN dựa vào vị trí gốc
    transform.position = startPos + offset;

    // 🔥 Rung mạnh khi crit
    if (shakeTimer > 0)
    {
      shakeTimer -= Time.deltaTime;
      transform.position += (Vector3)Random.insideUnitCircle * critShakeIntensity;
    }
  }
}
