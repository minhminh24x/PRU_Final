using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI tutorialText;
    public GameObject portalGate;

    [Header("Tutorial Enemy")]
    [Tooltip("Kéo con quái trong Tutorial scene vào đây")]
    public GameObject enemyToKill;

    private int currentStep = 0;
    private bool enemyKilled = false;

    void Start()
    {
        if (portalGate != null) portalGate.SetActive(false);
        UpdateText();

        // Lắng nghe sự kiện quái chết
        if (enemyToKill != null)
        {
            var health = enemyToKill.GetComponent<EnemyHealth>();
            if (health != null)
                health.OnDied += OnEnemyKilled;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        switch (currentStep)
        {
            case 0: // Di chuyển (A hoặc D)
                if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
                    NextStep();
                break;

            case 1: // Nhảy (Space)
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    NextStep();
                break;

            case 2: // Tấn công (J) — chỉ chuyển step khi nhấn J
                if (Keyboard.current.jKey.wasPressedThisFrame)
                    NextStep();
                break;

            case 3: // Chờ quái chết
                if (enemyKilled || enemyToKill == null)
                    NextStep();
                break;

            case 4: // Nhặt đồ rớt
                // Tự động chuyển sau 3 giây
                break;
        }
    }

    void OnEnemyKilled()
    {
        enemyKilled = true;
    }

    void NextStep()
    {
        currentStep++;
        UpdateText();
    }

    void UpdateText()
    {
        if (tutorialText == null) return;

        switch (currentStep)
        {
            case 0:
                tutorialText.text = "Bấm phím <color=yellow>[A]</color> hoặc <color=yellow>[D]</color> để di chuyển.";
                break;
            case 1:
                tutorialText.text = "Tốt lắm! Bấm <color=yellow>[Space]</color> để nhảy lên các bục.";
                break;
            case 2:
                tutorialText.text = "Phía trước có quái! Bấm <color=yellow>[J]</color> để vung kiếm tấn công.";
                break;
            case 3:
                tutorialText.text = "Tiêu diệt con quái để tiếp tục!";
                break;
            case 4:
                tutorialText.text = "<color=yellow>Nhặt vật phẩm rơi ra!</color> Bình máu, mana và vàng.";
                // Mở portal sau 3 giây
                Invoke(nameof(OpenPortal), 3f);
                break;
            case 5:
                tutorialText.text = "Hoàn thành huấn luyện! <color=green>Cánh cổng đã mở.</color>";
                break;
        }
    }

    void OpenPortal()
    {
        currentStep = 5;
        UpdateText();
        if (portalGate != null) portalGate.SetActive(true);
    }

    void OnDestroy()
    {
        if (enemyToKill != null)
        {
            var health = enemyToKill.GetComponent<EnemyHealth>();
            if (health != null)
                health.OnDied -= OnEnemyKilled;
        }
    }
}