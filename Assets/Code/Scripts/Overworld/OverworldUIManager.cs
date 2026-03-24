using UnityEngine;
using TMPro;

public class OverworldUIManager : MonoBehaviour
{
    [Header("Panels (Bảng Pop-up)")]
    public GameObject statsPanel;
    public GameObject inventoryPanel;

    [Header("Hints (Chữ chỉ dẫn)")]
    public GameObject statsHintText; // Kéo chữ "Stats (Tab)" vào đây
    public GameObject inventoryHintText; // Kéo chữ "Inventory (E)" vào đây

    [Header("Node Info Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    void Update()
    {
        // Nhấn Tab để bật/tắt bảng Chỉ số
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (statsPanel != null) 
            {
                bool isOpening = !statsPanel.activeSelf;
                statsPanel.SetActive(isOpening);
                
                // Bảng mở ra (true) -> Chữ chỉ dẫn tắt đi (false) và ngược lại
                if (statsHintText != null) statsHintText.SetActive(!isOpening); 
            }
        }

        // Nhấn I để bật/tắt bảng Túi đồ
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel != null) 
            {
                bool isOpening = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(isOpening);
                
                if (inventoryHintText != null) inventoryHintText.SetActive(!isOpening);
            }
        }
    }

    public void UpdateNodeDisplay(string nodeName, string nodeDesc)
    {
        if (nameText != null) nameText.text = nodeName;
        if (descriptionText != null) descriptionText.text = nodeDesc;
    }
}