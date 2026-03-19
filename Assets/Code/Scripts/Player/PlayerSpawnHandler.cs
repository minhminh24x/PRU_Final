using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerSpawnHandler : MonoBehaviour
{
  Rigidbody2D rb;
  Collider2D col;

  [Header("Ground")]
  public LayerMask groundLayer;
  public float raycastHeight = 5f;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
      // Nếu là các Scene menu/map ngoài thì KHÔNG chạy logic Spawn tìm mặt đất
      if (scene.name == "overWorld" || scene.name == "menuGame" || scene.name == "Cutscene" || scene.name == "loginPage")
      {
          return; 
      }

      StartCoroutine(SpawnRoutine());
  }

  IEnumerator SpawnRoutine()
  {
    // Đợi physics + tilemap load xong
    yield return new WaitForFixedUpdate();
    yield return new WaitForEndOfFrame();

    GameObject spawn = GameObject.FindWithTag("PlayerSpawn");
    if (spawn == null)
    {
      Debug.LogError("❌ KHÔNG TÌM THẤY PlayerSpawn");
      yield break;
    }

    // TẠM TẮT PHYSICS
    rb.linearVelocity = Vector2.zero;
    rb.angularVelocity = 0f;
    rb.simulated = false;

    // RAYCAST XUỐNG TÌM MẶT ĐẤT
    Vector2 rayStart = spawn.transform.position + Vector3.up * raycastHeight;
    RaycastHit2D hit = Physics2D.Raycast(
        rayStart,
        Vector2.down,
        raycastHeight * 2f,
        groundLayer
    );

    if (hit)
    {
      float footOffset = col.bounds.extents.y;

      Vector2 finalPos = hit.point + Vector2.up * footOffset;
      rb.position = finalPos;

      Debug.Log("✅ Player spawned ON GROUND by Raycast");
    }
    else
    {
      // fallback (hiếm)
      rb.position = spawn.transform.position;
      Debug.LogWarning("⚠️ Không raycast được ground, dùng spawn trực tiếp");
    }

    // BẬT LẠI PHYSICS
    rb.simulated = true;
    rb.Sleep();
  }
}
