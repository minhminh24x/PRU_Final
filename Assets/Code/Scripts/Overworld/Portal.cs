using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Cấu hình dịch chuyển")]
    public string sceneToLoad = "overWorld"; // Tên scene Overworld của bạn

    private void OnTriggerEnter2D(Collider2D other)
{
    // Cứ có cái gì chạm vào là phải hiện tên thằng đó lên!
    Debug.Log("Đã phát hiện vật thể: " + other.name);
    
    // Sau đó mới kiểm tra Tag
    if (other.CompareTag("Player"))
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
}