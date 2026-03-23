using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Message")]
    public TextMeshProUGUI notificationText; // Kéo Text cảnh báo vào đây trên Unity Editor

    // Nút Play - New Game
    public void PlayGame()
    {
        if (notificationText != null) notificationText.text = "";

        // Reset dữ liệu GameSession (giữ nguyên thông tin đăng nhập)
        GameSession.CurrentLevel = 1; 
        GameSession.Coins = 0; 
        GameSession.Score = 0;
        GameSession.CurrentHealth = 100; 
        GameSession.MaxHealth = 100;
        GameSession.EnemiesKilled = 0; 
        GameSession.TotalPlayTime = 0;

        GameSession.PlayerLevel = 1; 
        GameSession.CurrentExp = 0; 
        GameSession.UnspentStatPoints = 0;
        GameSession.BaseDEF = 100; 
        GameSession.BaseINT = 50; 
        GameSession.BaseSTR = 10; 
        GameSession.BaseAGI = 5;
        GameSession.ExtraDEF = 0; 
        GameSession.ExtraINT = 0; 
        GameSession.ExtraSTR = 0; 
        GameSession.ExtraAGI = 0;
        GameSession.IsNewGame = true;

        PlayerPrefs.DeleteAll();

        SceneManager.LoadSceneAsync("Cutscene");
    }

    // Nút Continue - Load Game
    public void ContinueGame()
    {
        if (GameSession.IsNewGame)
        {
            if (notificationText != null)
            {
                notificationText.text = "Bạn không có dữ liệu cũ! Vui lòng nhấn nút <color=yellow>Play (New Game)</color> để tạo mới.";
                StartCoroutine(ClearNotification());
            }
            Debug.Log("<color=red>Không có dữ liệu cũ! Vui lòng chọn Play (New Game).</color>");
            return;
        }

        if (notificationText != null) notificationText.text = "";

        // Chuyển tới level cuối cùng người chơi đang ở
        if (!string.IsNullOrEmpty(GameSession.CurrentScene) && GameSession.CurrentScene != "SampleScene" && GameSession.CurrentScene != "menuGame")
        {
            SceneManager.LoadScene(GameSession.CurrentScene);
        }
        else
        {
            SceneManager.LoadScene("overWorld");
        }
    }

    private IEnumerator ClearNotification()
    {
        yield return new WaitForSeconds(3f);
        if (notificationText != null) notificationText.text = "";
    }


    // Nút Options - Mở bảng setting
    public void OpenOptions()
    {
        Debug.Log("Open Options");

        // Ví dụ: bật panel Options
        GameObject optionsPanel = GameObject.Find("OptionsPanel");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    // Nút Quit
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
