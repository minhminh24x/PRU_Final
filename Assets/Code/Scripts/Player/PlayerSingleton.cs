using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Kiểm tra xem có phải cảnh "Hòa bình" (không cần đánh nhau) không
        // Thêm menuGame, Cutscene và loginPage vào danh sách tàng hình
        bool hidePlayer = (scene.name == "overWorld" || scene.name == "menuGame" || scene.name == "Cutscene" || scene.name == "loginPage");

        // 2. Chỉ tắt "Vỏ" (Renderer) và "Va chạm" (Collider), KHÔNG tắt cả Object
        var renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null) renderer.enabled = !hidePlayer;

        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = !hidePlayer;

        // 3. Tắt luôn Canvas thanh máu nếu đang ở Overworld hoặc Menu
        var canvas = GetComponentInChildren<Canvas>();
        if (canvas != null) canvas.enabled = !hidePlayer;

        // 4. Di chuyển nhân vật về điểm Spawn nếu về lại map chiến đấu
        if (!hidePlayer)
        {
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}