using UnityEngine;
using UnityEngine.UI; 

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Slider slider;

    // Hàm Init làm cầu nối để tương thích với code PlayerHealth và BossHealth hiện tại
    public void Init(int health)
    {
        SetMaxHealth(health);
    }

    public void SetMaxHealth(int health)
    {
        if (slider == null) return;
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        if (slider == null) return;
        slider.value = health;
    }
}