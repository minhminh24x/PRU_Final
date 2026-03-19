using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugGoToBoss : MonoBehaviour
{
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.B)) // nhấn B
    {
      SceneManager.LoadScene("Scene_Boss");
    }
  }
}
