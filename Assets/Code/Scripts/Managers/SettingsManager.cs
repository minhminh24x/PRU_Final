using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public string masterVolumeParam = "MasterVol";
    public string bgmVolumeParam = "BGMVol";
    public string sfxVolumeParam = "SFXVol";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        // Volume is 0 to 1 from slider, convert to decibels for Mixer
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        if (mainMixer != null)
        {
            mainMixer.SetFloat(masterVolumeParam, db);
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        if (mainMixer != null)
        {
            mainMixer.SetFloat(bgmVolumeParam, db);
        }
        PlayerPrefs.SetFloat("BGMVolume", volume);
        
        // Notify BGMManager if not using Mixer
        if (BGMManager.Instance != null && mainMixer == null)
        {
            BGMManager.Instance.UpdateVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        if (mainMixer != null)
        {
            mainMixer.SetFloat(sfxVolumeParam, db);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }
}
