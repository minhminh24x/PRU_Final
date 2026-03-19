using UnityEngine;
using MongoDB.Driver;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    [Header("Auto Save")]
    public float autoSaveInterval = 60f;

    float _timer;

    void Update()
    {
        if (!GameSession.IsLoggedIn) return;

        GameSession.TotalPlayTime += Time.deltaTime;

        _timer += Time.deltaTime;
        if (_timer >= autoSaveInterval)
        {
            _timer = 0;
            SaveProgress();
        }
    }

    // === GỌI TỪ BẤT KỲ ĐÂU ===
    public async void SaveProgress()
    {
        if (!GameSession.IsLoggedIn) return;

        try
        {
            var db = MongoDbManager.Instance;
            var doc = GameSession.ToDocument();

            // Tìm và update, nếu chưa có thì insert
            var filter = Builders<ProgressDocument>.Filter.Eq(p => p.UserId, GameSession.UserId);
            var existing = await db.Progress.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                doc.Id = existing.Id;
                await db.Progress.ReplaceOneAsync(filter, doc);
            }
            else
            {
                await db.Progress.InsertOneAsync(doc);
            }

            Debug.Log("<color=green>✔ Đã lưu tiến trình!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Lưu thất bại: " + e.Message);
        }
    }

    // Save khi thoát game
    void OnApplicationQuit()
    {
        SaveProgress();
    }
}
