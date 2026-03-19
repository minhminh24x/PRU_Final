using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUserUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI helloText;
    public Button logoutButton;

    void Start()
    {
        // Hiện "Hello, Username"
        if (helloText != null && GameSession.IsLoggedIn)
        {
            helloText.text = "Hello, " + GameSession.Username;
        }

        // Nút Logout
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(OnLogout);
        }
    }

    void OnLogout()
    {
        // Save trước khi logout (nếu có PlayerDataManager)
        var dataManager = FindObjectOfType<PlayerDataManager>();
        if (dataManager != null)
            dataManager.SaveProgress();

        // Xóa session
        GameSession.Logout();

        // Quay về login
        SceneManager.LoadScene("loginPage");
    }
}
