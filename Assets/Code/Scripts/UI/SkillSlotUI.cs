using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý giao diện của ô Skill.
/// </summary>
public class SkillSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image skillIcon;
    public Image cooldownOverlay;     // Image dạng Filled
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI cooldownText;

    [Header("Skill Settings")]
    public Sprite iconSprite;
    public int manaCost = 20;

    void Start()
    {
        // Khởi tạo icon và giá mana
        if (skillIcon != null && iconSprite != null)
        {
            skillIcon.sprite = iconSprite;
            skillIcon.color = Color.white;
        }

        if (manaCostText != null)
        {
            manaCostText.text = manaCost.ToString();
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0;
        }

        if (cooldownText != null)
        {
            cooldownText.text = "";
        }
    }

    /// <summary>
    /// Gọi hàm này từ PlayerCombat (nếu có update thời gian hồi chiêu)
    /// </summary>
    public void UpdateCooldown(float currentCooldown, float maxCooldown)
    {
        if (cooldownOverlay != null)
        {
            if (currentCooldown > 0)
            {
                cooldownOverlay.fillAmount = currentCooldown / maxCooldown;
                if (cooldownText != null) 
                    cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
            }
            else
            {
                cooldownOverlay.fillAmount = 0;
                if (cooldownText != null) 
                    cooldownText.text = "";
            }
        }
    }
}
