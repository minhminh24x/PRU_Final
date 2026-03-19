using UnityEngine;
using MongoDB.Driver;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class MongoDbManager : MonoBehaviour
{
    public static MongoDbManager Instance { get; private set; }

    [Header("MongoDB Atlas Connection")]
    [Tooltip("Dán connection string từ MongoDB Atlas vào đây")]
    public string connectionString = "mongodb+srv://gameadmin:abc123456@gamecluster.xxxxx.mongodb.net/?retryWrites=true&w=majority";
    public string databaseName = "GameDB";

    IMongoDatabase _database;

    public IMongoCollection<UserDocument> Users
        => _database.GetCollection<UserDocument>("Users");

    public IMongoCollection<ProgressDocument> Progress
        => _database.GetCollection<ProgressDocument>("PlayerProgress");

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Kết nối MongoDB với SSL bypass (cần cho Unity Mono runtime)
        try
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);

            // Bypass SSL certificate verification - Unity's Mono TLS
            // không xử lý được certificate của MongoDB Atlas
            settings.SslSettings = new SslSettings
            {
                CheckCertificateRevocation = false,
                ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
            settings.AllowInsecureTls = true;

            var client = new MongoClient(settings);
            _database = client.GetDatabase(databaseName);
            Debug.Log("<color=green>Kết nối MongoDB thành công!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Kết nối MongoDB thất bại: " + e.Message);
        }
    }
}
