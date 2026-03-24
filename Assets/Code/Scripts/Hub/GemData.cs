using UnityEngine;

/// <summary>
/// ScriptableObject chứa dữ liệu của một viên ngọc nguyên tố.
/// Tạo tài sản qua: Assets > Create > PRU > GemData
/// </summary>
[CreateAssetMenu(fileName = "GemData_", menuName = "PRU/GemData")]
public class GemData : ScriptableObject
{
    [Header("Thông tin ngọc")]
    public GemType gemType;
    public string gemName = "Ngọc Thiên Nhiên";
    public Sprite gemSprite;        // sprite ngọc hiện trên bệ sau khi nhặt
    public Color gemColor = Color.green;

    [Header("Lore Bệ Đá — Chưa thu thập")]
    [Tooltip("Hiển thị khi boss chưa bị đánh hoặc ngọc chưa nhặt.")]
    [TextArea(2, 5)]
    public string[] pedestalLore;

    [Header("Lore Bệ Đá — Đã đặt ngọc")]
    [Tooltip("Hiển thị khi ngọc đã được đặt lên bệ.")]
    [TextArea(2, 5)]
    public string[] obtainedLore;
}
