using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Xử lý các sự kiện sau khi Boss bị tiêu diệt:
/// 1. Rớt ngọc (Gem)
/// 2. Chờ 10s rồi chuyển về Hub/Overworld.
/// </summary>
public class BossDefeatHandler : MonoBehaviour
{
    [Header("Phần thưởng")]
    public GameObject gemPrefab;
    public Transform dropPoint;

    [Header("Chuyển cảnh")]
    public string targetSceneName = "overWorld";
    public float delayBeforeTransition = 10f;

    private EnemyHealth _health;
    private bool _hasTriggered = false;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleBossDeath;
    }

    void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleBossDeath;
    }

    private void HandleBossDeath()
    {
        if (_hasTriggered) return;
        _hasTriggered = true;

        Debug.Log("<color=green>[BossDefeat] Boss has been defeated!</color>");

        // 1. Rớt ngọc
        SpawnReward();

        // 2. Bắt đầu đếm ngược chuyển cảnh
        StartCoroutine(TransitionRoutine());
    }

    private void SpawnReward()
    {
        if (gemPrefab == null)
        {
            Debug.LogWarning("[BossDefeat] Chưa gán Gem Prefab!");
            return;
        }

        Vector3 pos = dropPoint != null ? dropPoint.position : transform.position;
        Instantiate(gemPrefab, pos, Quaternion.identity);
        Debug.Log("[BossDefeat] Đã rớt ngọc tại: " + pos);
    }

    private IEnumerator TransitionRoutine()
    {
        Debug.Log($"[BossDefeat] Chuẩn bị chuyển cảnh về {targetSceneName} sau {delayBeforeTransition} giây...");
        
        yield return new WaitForSeconds(delayBeforeTransition);

        Debug.Log("[BossDefeat] Đang chuyển cảnh với hiệu ứng Fade...");
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.EnterSceneWithTransition(targetSceneName, "BOSS DEFEATED!\nReturning to Overworld...");
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
