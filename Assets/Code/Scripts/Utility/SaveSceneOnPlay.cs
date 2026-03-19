using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSceneOnPlay : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
    }
}
