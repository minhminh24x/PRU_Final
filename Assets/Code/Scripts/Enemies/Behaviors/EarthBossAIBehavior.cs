using UnityEngine;

[CreateAssetMenu(fileName = "EarthBossBehavior", menuName = "Game/AI/Earth Boss Behavior")]
public class EarthBossAIBehavior : EnemyAIBehavior
{
    public override void Execute(EnemyController controller)
    {
        // Hiện tại EarthBossController tự chạy logic trong Update của nó.
        // ScriptableObject này chỉ đóng vai trò là "mỏ neo" để gán vào EnemyData.
        
        var bossCtrl = controller.GetComponent<EarthBossController>();
        if (bossCtrl == null)
        {
            // Tự động add nếu chưa có (Hưng đỡ phải kéo tay)
            bossCtrl = controller.gameObject.AddComponent<EarthBossController>();
        }
        
        // Nếu muốn di chuyển khi không action, có thể thêm logic ở đây
        // Nhưng Rannoch được mô tả là "Chậm chạp", nên Chase thường xuyên là đủ.
    }
}
