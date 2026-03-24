using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton DontDestroyOnLoad quản lý trạng thái 4 viên ngọc.
/// Lưu/load qua PlayerPrefs để persist giữa các lần chơi.
/// </summary>
public class GemManager : MonoBehaviour
{
    public static GemManager Instance { get; private set; }

    private const int GEM_COUNT = 4;
    private const string PREF_PREFIX = "gem_collected_";

    // Index = (int)GemType
    private bool[] gemsCollected = new bool[GEM_COUNT];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPrefs();
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>Đánh dấu một viên ngọc đã được thu thập.</summary>
    public void CollectGem(GemData gemData)
    {
        if (gemData == null) return;

        int idx = (int)gemData.gemType;
        if (gemsCollected[idx])
        {
            Debug.Log($"[GemManager] Ngọc {gemData.gemName} đã thu thập trước đó.");
            return;
        }

        gemsCollected[idx] = true;
        SaveToPrefs(idx);

        Debug.Log($"[GemManager] Thu thập: {gemData.gemName} ({gemData.gemType})");

        // Thông báo tất cả AncientPedestal trong scene cập nhật UI
        RefreshAllPedestals();

        // Kiểm tra đủ 4 ngọc
        CheckAllGemsCollected();
    }

    /// <summary>Trả về true nếu ngọc loại này đã thu thập.</summary>
    public bool IsCollected(GemType gemType)
    {
        return gemsCollected[(int)gemType];
    }

    /// <summary>Trả về số ngọc đã thu thập.</summary>
    public int CollectedCount()
    {
        int count = 0;
        foreach (var g in gemsCollected)
            if (g) count++;
        return count;
    }

    /// <summary>Reset toàn bộ ngọc (dùng cho New Game).</summary>
    public void ResetAllGems()
    {
        for (int i = 0; i < GEM_COUNT; i++)
        {
            gemsCollected[i] = false;
            PlayerPrefs.SetInt(PREF_PREFIX + i, 0);
        }
        PlayerPrefs.Save();
    }

    // ─── Internal ─────────────────────────────────────────────────────────────

    void CheckAllGemsCollected()
    {
        if (CollectedCount() < GEM_COUNT) return;

        Debug.Log("[GemManager] Đủ 4 ngọc! Kích hoạt cổng trung tâm.");

        if (AncientGateManager.Instance != null)
            AncientGateManager.Instance.ActivateGate();
        else
            Debug.LogWarning("[GemManager] Không tìm thấy AncientGateManager trong scene!");
    }

    void RefreshAllPedestals()
    {
        // Có thể không ở Hub scene → FindObjectsByType an toàn khi không có
        var pedestals = FindObjectsByType<AncientPedestal>(FindObjectsSortMode.None);
        foreach (var p in pedestals)
            p.RefreshGemVisual();
    }

    void LoadFromPrefs()
    {
        for (int i = 0; i < GEM_COUNT; i++)
            gemsCollected[i] = PlayerPrefs.GetInt(PREF_PREFIX + i, 0) == 1;
    }

    void SaveToPrefs(int idx)
    {
        PlayerPrefs.SetInt(PREF_PREFIX + idx, 1);
        PlayerPrefs.Save();
    }
}
