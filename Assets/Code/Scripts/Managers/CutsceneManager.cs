using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "Tutorial"; // Tên scene sẽ load tiếp theo

    void Start()
    {
        // Gắn sự kiện: Khi video chạy đến cuối (loopPointReached), tự động gọi hàm EndReach
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += EndReach;
        }
    }

    // Hàm tự động kích hoạt khi video kết thúc
    void EndReach(VideoPlayer vp)
    {
        LoadNextScene();
    }

    // Hàm này sẽ được gán vào nút Skip trên UI
    public void SkipCutscene()
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        // Chuyển sang map huấn luyện
        SceneManager.LoadScene(nextSceneName);
    }
}