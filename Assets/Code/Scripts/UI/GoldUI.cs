using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void OnEnable()
    {
        // Lắng nghe sự kiện tiền thay đổi
        CurrencyManager.OnGoldChanged += UpdateGoldUI;
        // Cập nhật ngay lần đầu bật lên
        UpdateGoldUI(CurrencyManager.Gold);
    }

    void OnDisable()
    {
        // Hủy lắng nghe để tránh lỗi memory leak khi đổi scene
        CurrencyManager.OnGoldChanged -= UpdateGoldUI;
    }

    void UpdateGoldUI(int currentGold)
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}";
        }
    }
}
