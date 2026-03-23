using UnityEngine;

public class BGMManager : MonoBehaviour
{
  public static BGMManager Instance => instance;
  private static BGMManager instance;

  private AudioSource audioSource;

  void Awake()
  {
    if (instance != null)
    {
      Destroy(gameObject);
      return;
    }

    instance = this;
    DontDestroyOnLoad(gameObject);

    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }
  }

  private void Start()
  {
      // Apply initial volume from SettingsManager (if it exists)
      if (SettingsManager.Instance != null)
      {
          UpdateVolume(PlayerPrefs.GetFloat("BGMVolume", 0.75f));
      }
  }

  public void UpdateVolume(float volume)
  {
      if (audioSource != null)
      {
          audioSource.volume = volume;
      }
  }
}
