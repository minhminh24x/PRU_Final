using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class CameraTransitionTrigger : MonoBehaviour
{
    [Header("Camera Transition Setup")]
    [Tooltip("Camera của Khu Vực TIẾP THEO (sẽ được Bật lên)")]
    [SerializeField] GameObject cameraToEnable;

    [Tooltip("Camera của Khu Vực CŨ (sẽ bị Tắt đi)")]
    [SerializeField] GameObject cameraToDisable;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ kích hoạt khi người đi qua là Player
        if (other.CompareTag("Player"))
        {
            if (cameraToEnable != null)
                cameraToEnable.SetActive(true);

            if (cameraToDisable != null)
                cameraToDisable.SetActive(false);
        }
    }
}
