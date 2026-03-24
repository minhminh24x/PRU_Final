using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

/// <summary>
/// Gắn script này vào cùng GameObject với CinemachineCamera.
/// Tự động re-link Tracking Target về PlayerSingleton sau mỗi lần load scene.
/// </summary>
[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineCameraAutoTarget : MonoBehaviour
{
    private CinemachineCamera _vcam;

    void Awake()
    {
        _vcam = GetComponent<CinemachineCamera>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        AssignTarget();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignTarget();
    }

    void AssignTarget()
    {
        // Ưu tiên 1: PlayerSingleton (DontDestroyOnLoad)
        if (PlayerSingleton.Instance != null)
        {
            _vcam.Target.TrackingTarget = PlayerSingleton.Instance.transform;
            _vcam.Follow = PlayerSingleton.Instance.transform;
            Debug.Log($"[CinemachineCameraAutoTarget] Đã gán target = PlayerSingleton");
            return;
        }

        // Ưu tiên 2: tìm qua tag
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _vcam.Target.TrackingTarget = player.transform;
            _vcam.Follow = player.transform;
            Debug.Log($"[CinemachineCameraAutoTarget] Đã gán target = Player (by tag)");
            return;
        }

        Debug.LogWarning("[CinemachineCameraAutoTarget] Không tìm thấy Player để gán target!");
    }

    void LateUpdate()
    {
        // Safety: nếu target bị mất giữa chừng thì tìm lại
        if (_vcam.Target.TrackingTarget == null)
            AssignTarget();
    }
}
