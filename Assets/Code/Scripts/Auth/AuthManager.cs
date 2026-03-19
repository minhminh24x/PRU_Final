using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MongoDB.Driver;
using System.Threading.Tasks;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("Login UI")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button     loginButton;
    public Button     goRegisterButton;
    public TextMeshProUGUI messageText;

    [Header("Register UI (nếu dùng chung scene)")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public TMP_InputField regUsernameInput;
    public TMP_InputField regPasswordInput;
    public TMP_InputField regEmailInput;
    public Button     registerButton;
    public Button     backToLoginButton;

    void Start()
    {
        // Ẩn ký tự mật khẩu
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        regPasswordInput.contentType = TMP_InputField.ContentType.Password;
        regPasswordInput.ForceLabelUpdate();

        loginButton.onClick.AddListener(OnLogin);
        goRegisterButton.onClick.AddListener(() => { loginPanel.SetActive(false); registerPanel.SetActive(true); });
        registerButton.onClick.AddListener(OnRegister);
        backToLoginButton.onClick.AddListener(() => { registerPanel.SetActive(false); loginPanel.SetActive(true); });

        // Bắt đầu ở Login panel
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    // ==================== ĐĂNG NHẬP ====================
    async void OnLogin()
    {
        string u = usernameInput.text.Trim();
        string p = passwordInput.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowMsg("Nhập đầy đủ thông tin!", Color.red);
            return;
        }

        loginButton.interactable = false;
        ShowMsg("Đang đăng nhập...", Color.white);

        try
        {
            // Tìm user trong MongoDB
            var db = MongoDbManager.Instance;
            var filter = Builders<UserDocument>.Filter.Eq(x => x.Username, u)
                       & Builders<UserDocument>.Filter.Eq(x => x.Password, p);

            var user = await db.Users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                ShowMsg("Sai tài khoản hoặc mật khẩu!", Color.red);
                loginButton.interactable = true;
                return;
            }

            // Login thành công → lưu session
            GameSession.SetLogin(user.Id, user.Username);

            // Load progress
            ShowMsg("Đang tải tiến trình...", Color.yellow);
            var progressFilter = Builders<ProgressDocument>.Filter.Eq(x => x.UserId, user.Id);
            var progress = await db.Progress.Find(progressFilter).FirstOrDefaultAsync();

            if (progress != null)
                GameSession.LoadFromDocument(progress);

            ShowMsg("Thành công!", Color.green);
            await Task.Delay(500);

            // Chuyển vào menu game
            SceneManager.LoadScene("menuGame");
            return; // Không truy cập UI nữa vì scene đã bị destroy
        }
        catch (System.Exception e)
        {
            ShowMsg("Lỗi: " + e.Message, Color.red);
            Debug.LogError(e);
        }

        loginButton.interactable = true;
    }

    // ==================== ĐĂNG KÝ ====================
    async void OnRegister()
    {
        string u = regUsernameInput.text.Trim();
        string p = regPasswordInput.text;
        string email = regEmailInput.text.Trim();

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowMsg("Nhập đầy đủ Username và Password!", Color.red);
            return;
        }

        registerButton.interactable = false;
        ShowMsg("Đang xử lý...", Color.white);

        try
        {
            var db = MongoDbManager.Instance;

            // Kiểm tra username đã tồn tại chưa
            var existing = await db.Users
                .Find(x => x.Username == u)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                ShowMsg("Username đã tồn tại!", Color.red);
                registerButton.interactable = true;
                return;
            }

            // Tạo user mới
            var newUser = new UserDocument
            {
                Username = u,
                Password = p,
                Email    = email
            };
            await db.Users.InsertOneAsync(newUser);

            // Tạo progress mặc định
            var defaultProgress = new ProgressDocument
            {
                UserId = newUser.Id
            };
            await db.Progress.InsertOneAsync(defaultProgress);

            ShowMsg("Đăng ký thành công! Quay về Login...", Color.green);
            await Task.Delay(1000);

            // Quay về Login panel
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        catch (System.Exception ex)
        {
            ShowMsg("Lỗi: " + ex.Message, Color.red);
            Debug.LogError(ex);
        }

        registerButton.interactable = true;
    }

    void ShowMsg(string msg, Color c)
    {
        if (messageText != null) { messageText.text = msg; messageText.color = c; }
    }
}
