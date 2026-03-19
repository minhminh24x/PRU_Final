using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Bắt buộc phải gọi lại bộ Input mới

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;
    bool isPaused;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false); 
        isPaused = false;
    }

    void Update()
    {
        bool escPressed = false;

        // Cơ chế bọc hậu: Kiểm tra CẢ 2 HỆ THỐNG PHÍM cùng lúc, thằng nào nhận cũng được!
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) 
            escPressed = true;
            
        if (Input.GetKeyDown(KeyCode.Escape)) 
            escPressed = true;

        if (escPressed)
        {
            // Bắn dòng chữ xanh này ra Console để báo cáo
            Debug.Log("<color=green>ĐÃ BẮM ESC! Trạng thái Pause: " + !isPaused + "</color>"); 
            
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToOW()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && GameManager.Instance != null)
            GameManager.Instance.SavePlayer(player.transform.position);

        Time.timeScale = 1f;
        SceneManager.LoadScene("overWorld");
    }

    public void QuitToMenu()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && GameManager.Instance != null)
            GameManager.Instance.SavePlayer(player.transform.position);

        Time.timeScale = 1f;
        SceneManager.LoadScene("menuGame");
    }
}