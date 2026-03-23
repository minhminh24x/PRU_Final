using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// === User document ===
public class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Email    { get; set; }
}

// === Player Progress document ===
public class ProgressDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId        { get; set; }

    // Tiến trình
    public int    CurrentLevel  { get; set; } = 1;
    public string CurrentScene  { get; set; } = "SampleScene";
    public float  PlayerX       { get; set; }
    public float  PlayerY       { get; set; }

    // Stats
    public int    Coins         { get; set; }
    public int    Score         { get; set; }
    public int    CurrentHealth { get; set; } = 100;
    public int    MaxHealth     { get; set; } = 100;
    public int    EnemiesKilled { get; set; }
    public float  TotalPlayTime { get; set; }

    // RPG Stats
    public int    PlayerLevel       { get; set; } = 1;
    public int    CurrentExp        { get; set; } = 0;
    public int    UnspentStatPoints { get; set; } = 0;
    public int    BaseDEF           { get; set; } = 100;
    public int    BaseINT           { get; set; } = 50;
    public int    BaseSTR           { get; set; } = 10;
    public int    BaseAGI           { get; set; } = 5;
    
    public int    ExtraDEF          { get; set; } = 0;
    public int    ExtraINT          { get; set; } = 0;
    public int    ExtraSTR          { get; set; } = 0;
    public int    ExtraAGI          { get; set; } = 0;
}
