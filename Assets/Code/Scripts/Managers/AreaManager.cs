using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class AreaManager : MonoBehaviour
{
    [Header("Enemies to Defeat")]
    [Tooltip("Kéo thả tất cả quái vật của khu vực (Room/Wave) này vào đây")]
    [SerializeField] EnemyHealth[] enemiesInArea;

    [Header("Optional Automation")]
    [Tooltip("Gắn Tick nếu bạn nhóm tất cả quái của khu này làm con của Object AreaManager này, code sẽ tự động tìm mà bạn không cần kéo tay")]
    [SerializeField] bool autoFindEnemiesInChildren = false;

    [Header("Barriers to Unlock (Optional)")]
    [Tooltip("Kéo thả các bức tường, chướng ngại vật cản đường vào đây. Chúng sẽ bị tắt (mở đường) khi quái chết hết.")]
    [SerializeField] GameObject[] barriers;

    [Header("Events (Kéo thả sự kiện nâng cao)")]
    [SerializeField] UnityEvent onAreaCleared;

    private int _enemiesRemaining;

    void Start()
    {
        // 1. Tự động gom quái nếu bật chức năng
        if (autoFindEnemiesInChildren)
        {
            enemiesInArea = GetComponentsInChildren<EnemyHealth>();
        }

        // 2. Chốt sổ số lượng
        _enemiesRemaining = enemiesInArea.Length;

        if (_enemiesRemaining == 0)
        {
            Debug.LogWarning($"[AreaManager] Cảnh báo: Khu vực {gameObject.name} không có con quái nào cả!");
            return;
        }

        // 3. Đăng ký nhận tin nhắn báo tử từ từng con quái (Event OnDied)
        foreach (var enemy in enemiesInArea)
        {
            if (enemy != null)
            {
                // Khi enemy này chết (gọi hàm Die()), nó sẽ tự động réo gọi hàm HandleEnemyDied() của Manager
                enemy.OnDied += HandleEnemyDied;
            }
        }

        Debug.Log($"[AreaManager] {gameObject.name} đã khóa! Cần giết {_enemiesRemaining} con quái để qua màn.");
    }

    void HandleEnemyDied()
    {
        _enemiesRemaining--;
        Debug.Log($"[AreaManager] {gameObject.name} => 1 quái chết! Còn lại: {_enemiesRemaining}/{enemiesInArea.Length}");

        if (_enemiesRemaining <= 0)
        {
            AreaCleared();
        }
    }

    void AreaCleared()
    {
        Debug.Log($"<color=green>[AreaManager] KHU VỰC {gameObject.name} ĐÃ BỊ CÀN QUÉT SẠCH!</color>");

        // Mở cửa / Xóa vật cản
        foreach (var barrier in barriers)
        {
            if (barrier != null)
            {
                // Bạn có thể đổi hàm này thành Play() animation mở cửa, ở đây làm tắt luôn vật thể
                barrier.SetActive(false); 
            }
        }

        // Chạy các sự kiện ngoài luồng nếu có (cộng thưởng, nổ pháo hoa...)
        if (onAreaCleared != null)
        {
            onAreaCleared.Invoke();
        }
    }
}
