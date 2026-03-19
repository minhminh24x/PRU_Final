using UnityEngine;

public static class GameSession
{
    // Login info
    public static string UserId;
    public static string Username;
    public static bool   IsLoggedIn;

    // Game progress (trong RAM)
    public static int    CurrentLevel   = 1;
    public static string CurrentScene   = "SampleScene";
    public static int    Coins          = 0;
    public static int    Score          = 0;
    public static int    CurrentHealth  = 100;
    public static int    MaxHealth      = 100;
    public static int    EnemiesKilled  = 0;
    public static float  TotalPlayTime  = 0f;

    public static void SetLogin(string userId, string username)
    {
        UserId = userId; Username = username; IsLoggedIn = true;
    }

    public static void LoadFromDocument(ProgressDocument doc)
    {
        CurrentLevel  = doc.CurrentLevel;
        CurrentScene  = doc.CurrentScene;
        Coins         = doc.Coins;
        Score         = doc.Score;
        CurrentHealth = doc.CurrentHealth;
        MaxHealth     = doc.MaxHealth;
        EnemiesKilled = doc.EnemiesKilled;
        TotalPlayTime = doc.TotalPlayTime;
    }

    public static ProgressDocument ToDocument()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        return new ProgressDocument
        {
            UserId        = UserId,
            CurrentLevel  = CurrentLevel,
            CurrentScene  = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            PlayerX       = player != null ? player.transform.position.x : 0,
            PlayerY       = player != null ? player.transform.position.y : 0,
            Coins         = Coins,
            Score         = Score,
            CurrentHealth = CurrentHealth,
            MaxHealth     = MaxHealth,
            EnemiesKilled = EnemiesKilled,
            TotalPlayTime = TotalPlayTime
        };
    }

    public static void Logout()
    {
        UserId = null; Username = null; IsLoggedIn = false;
        CurrentLevel = 1; Coins = 0; Score = 0;
        CurrentHealth = 100; EnemiesKilled = 0; TotalPlayTime = 0;
    }
}
