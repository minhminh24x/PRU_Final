using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Nút Play - New Game
    public void PlayGame()
    {
        // Nếu muốn reset dữ liệu, có thể clear ở đây
        PlayerPrefs.DeleteAll();

        SceneManager.LoadSceneAsync("Cutscene");
    }

    // Nút Continue - Load Game
    public void ContinueGame()
    {
        SceneManager.LoadScene("overWorld");
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
