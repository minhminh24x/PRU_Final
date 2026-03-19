using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsUIManager : MonoBehaviour
{
    [Header("Player Reference")]
    private PlayerStats playerStats;

    [Header("Left Panel - Thông tin chung")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI unspentPointsText;
    
    [Header("Sliders (Thanh dài)")]
    public Slider strSlider;
    public Slider intSlider;
    public Slider defSlider;
    public Slider agiSlider;

    [Header("Texts (Hiển thị số cho rõ ràng)")]
    public TextMeshProUGUI strText;
    public TextMeshProUGUI intText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI agiText;

    void OnEnable()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            UpdateUI();
        }
    }

  public void UpdateUI()
    {
        if (playerStats == null) return;

        if (levelText != null) levelText.text = "Level " + playerStats.level;
        
        // Cập nhật UX: Hiển thị rõ EXP hiện tại / EXP cần để lên cấp
        if (expText != null) expText.text = $"XP: {playerStats.currentExp} / {playerStats.GetNextLevelExp()}";
        
        if (unspentPointsText != null) unspentPointsText.text = "Điểm dư: " + playerStats.unspentStatPoints;

       if (strSlider != null) { strSlider.maxValue = 20; strSlider.value = playerStats.extraSTR; }
        if (intSlider != null) { intSlider.maxValue = 20; intSlider.value = playerStats.extraINT; }
        if (defSlider != null) { defSlider.maxValue = 20; defSlider.value = playerStats.extraDEF; }
        if (agiSlider != null) { agiSlider.maxValue = 20; agiSlider.value = playerStats.extraAGI; }

        // Chỉ hiển thị số điểm đã cộng (biến extra)
        if (strText != null) strText.text = playerStats.extraSTR.ToString();
        if (intText != null) intText.text = playerStats.extraINT.ToString();
        if (defText != null) defText.text = playerStats.extraDEF.ToString();
        if (agiText != null) agiText.text = playerStats.extraAGI.ToString();
    }

    public void OnClickAddSTR()
    {
        if (playerStats != null && playerStats.unspentStatPoints > 0)
        {
            playerStats.unspentStatPoints--;
            playerStats.extraSTR++;
            playerStats.Recalculate();
            UpdateUI();
        }
    }

    public void OnClickAddINT()
    {
        if (playerStats != null && playerStats.unspentStatPoints > 0)
        {
            playerStats.unspentStatPoints--;
            playerStats.extraINT++;
            playerStats.Recalculate();
            
            // Đồng bộ thanh Mana mới
            playerStats.GetComponent<PlayerHealth>()?.UpdateAllUI();
            UpdateUI();
        }
    }

    public void OnClickAddDEF()
    {
        if (playerStats != null && playerStats.unspentStatPoints > 0)
        {
            playerStats.unspentStatPoints--;
            playerStats.extraDEF++;
            playerStats.Recalculate();
            
            // Đồng bộ thanh Máu mới
            playerStats.GetComponent<PlayerHealth>()?.UpdateAllUI();
            UpdateUI();
        }
    }

    public void OnClickAddAGI()
    {
        if (playerStats != null && playerStats.unspentStatPoints > 0)
        {
            playerStats.unspentStatPoints--;
            playerStats.extraAGI++;
            playerStats.Recalculate();
            UpdateUI();
        }
    }
}