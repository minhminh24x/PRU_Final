using System;
using UnityEngine;

/// <summary>
/// Hệ thống tiền tệ — static, tồn tại suốt game.
/// UI hoặc script khác lắng nghe OnGoldChanged để cập nhật.
/// </summary>
public static class CurrencyManager
{
    public static int Gold { get; private set; } = 0;

    public static event Action<int> OnGoldChanged; // truyền gold hiện tại

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
        Debug.Log($"<color=yellow>+{amount} Gold! Tổng: {Gold}</color>");
        OnGoldChanged?.Invoke(Gold);
    }

    public static bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (Gold < amount) return false;

        Gold -= amount;
        Debug.Log($"<color=orange>-{amount} Gold! Còn lại: {Gold}</color>");
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public static bool HasEnough(int amount)
    {
        return Gold >= amount;
    }

    /// <summary>Reset về 0 (khi new game hoặc logout)</summary>
    public static void Reset()
    {
        Gold = 0;
        OnGoldChanged?.Invoke(Gold);
    }

    /// <summary>Set trực tiếp (khi load từ MongoDB)</summary>
    public static void SetGold(int value)
    {
        Gold = Mathf.Max(0, value);
        OnGoldChanged?.Invoke(Gold);
    }
}
