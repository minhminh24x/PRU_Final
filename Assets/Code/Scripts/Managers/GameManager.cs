using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Vector3 lastCheckpointPosition;
    [HideInInspector] public GameObject player;

    private void Awake()
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

    // Gọi mỗi khi load scene/map mới
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdatePlayerReference();
        if (player != null)
        {
            lastCheckpointPosition = player.transform.position;
            Debug.Log("Scene loaded. Checkpoint position reset: " + lastCheckpointPosition);
        }
        else
        {
            Debug.LogWarning("Player not found when loading scene: " + scene.name);
        }
    }

    // Cập nhật lại reference tới player
    public void UpdatePlayerReference()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Set checkpoint thủ công khi qua flag, qua cửa, ...
    public void SetCheckpoint(Vector3 checkpointPos)
    {
        lastCheckpointPosition = checkpointPos;
        Debug.Log("Set new checkpoint: " + lastCheckpointPosition);
    }

    // Hồi sinh về checkpoint hiện tại
    public void RespawnPlayer()
    {
        UpdatePlayerReference();
        if (player != null)
        {
            player.transform.position = lastCheckpointPosition;
            Debug.Log("Respawn checkpoint position: " + lastCheckpointPosition);
            var damageable = player.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.IsAlive = true;
            }

            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ResetActionState();
            }
        }
        else
        {
            Debug.LogError("player null in RespawnPlayer");
        }
    }

    // Hồi sinh và hồi full máu tại checkpoint
    public void RevivePlayerAtCheckpoint()
    {
        UpdatePlayerReference();
        if (player != null)
        {
            player.transform.position = lastCheckpointPosition;

            var damageable = player.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.Health = damageable.MaxHealth;
                damageable.IsAlive = true;
            }

            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ResetActionState();
            }
        }
        else
        {
            Debug.LogError("player null in RevivePlayerAtCheckpoint");
        }
    }

    public void SavePlayer(Vector3 position)
    {
        // Cập nhật tọa độ vào GameSession hoặc trực tiếp gọi save
        Debug.Log($"Saving player at {position}");
        // Giả sử ta muốn lưu tọa độ này vào GameSession trước khi scene load
        GameSession.ToDocument(); // ToDocument tự tìm player và lấy stats
        // Trong project này, việc Save thực sự thường được gọi qua SaveGameManager (nếu có)
        // Hoặc đơn giản là log để không lỗi build.
    }

    // Đảm bảo giải phóng event khi đóng game
    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
