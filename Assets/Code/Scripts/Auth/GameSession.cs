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

    // Player RPG Stats
    public static int    PlayerLevel       = 1;
    public static int    CurrentExp        = 0;
    public static int    UnspentStatPoints = 0;
    public static int    BaseDEF           = 100;
    public static int    BaseINT           = 50;
    public static int    BaseSTR           = 10;
    public static int    BaseAGI           = 5;
    public static int    ExtraDEF          = 0;
    public static int    ExtraINT          = 0;
    public static int    ExtraSTR          = 0;
    public static int    ExtraAGI          = 0;

    // Flag để đánh dấu save mới tạo
    public static bool   IsNewGame         = true;

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

        PlayerLevel       = doc.PlayerLevel;
        CurrentExp        = doc.CurrentExp;
        UnspentStatPoints = doc.UnspentStatPoints;
        BaseDEF           = doc.BaseDEF;
        BaseINT           = doc.BaseINT;
        BaseSTR           = doc.BaseSTR;
        BaseAGI           = doc.BaseAGI;
        ExtraDEF          = doc.ExtraDEF;
        ExtraINT          = doc.ExtraINT;
        ExtraSTR          = doc.ExtraSTR;
        ExtraAGI          = doc.ExtraAGI;

        // Nếu TotalPlayTime > 0 thì game này không phải là New Game
        IsNewGame         = TotalPlayTime <= 0f;
    }

    public static ProgressDocument ToDocument()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var hp = player.GetComponent<PlayerHealth>();
            var stats = player.GetComponent<PlayerStats>();
            if (hp != null)
            {
                CurrentHealth = hp.currentHP;
            }
            if (stats != null)
            {
                PlayerLevel = stats.level;
                CurrentExp = stats.currentExp;
                UnspentStatPoints = stats.unspentStatPoints;
                BaseDEF = stats.baseDEF;
                BaseINT = stats.baseINT;
                BaseSTR = stats.baseSTR;
                BaseAGI = stats.baseAGI;
                ExtraDEF = stats.extraDEF;
                ExtraINT = stats.extraINT;
                ExtraSTR = stats.extraSTR;
                ExtraAGI = stats.extraAGI;
            }
        }

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
            TotalPlayTime = TotalPlayTime,
            
            PlayerLevel       = PlayerLevel,
            CurrentExp        = CurrentExp,
            UnspentStatPoints = UnspentStatPoints,
            BaseDEF           = BaseDEF,
            BaseINT           = BaseINT,
            BaseSTR           = BaseSTR,
            BaseAGI           = BaseAGI,
            ExtraDEF          = ExtraDEF,
            ExtraINT          = ExtraINT,
            ExtraSTR          = ExtraSTR,
            ExtraAGI          = ExtraAGI
        };
    }

    public static void Logout()
    {
        UserId = null; Username = null; IsLoggedIn = false;
        CurrentLevel = 1; Coins = 0; Score = 0;
        CurrentHealth = 100; EnemiesKilled = 0; TotalPlayTime = 0;
        PlayerLevel = 1; CurrentExp = 0; UnspentStatPoints = 0;
        BaseDEF = 100; BaseINT = 50; BaseSTR = 10; BaseAGI = 5;
        ExtraDEF = 0; ExtraINT = 0; ExtraSTR = 0; ExtraAGI = 0;
        IsNewGame = true;
    }
}
