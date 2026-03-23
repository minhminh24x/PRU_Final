using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Master, BGM, SFX }
    public VolumeType volumeType;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null) return;

        // Load current value from PlayerPrefs
        float initialValue = 0.75f;
        switch (volumeType)
        {
            case VolumeType.Master: initialValue = PlayerPrefs.GetFloat("MasterVolume", 0.75f); break;
            case VolumeType.BGM: initialValue = PlayerPrefs.GetFloat("BGMVolume", 0.75f); break;
            case VolumeType.SFX: initialValue = PlayerPrefs.GetFloat("SFXVolume", 0.75f); break;
        }
        
        slider.value = initialValue;

        // Add listener
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        if (SettingsManager.Instance == null) return;

        switch (volumeType)
        {
            case VolumeType.Master:
                SettingsManager.Instance.SetMasterVolume(value);
                break;
            case VolumeType.BGM:
                SettingsManager.Instance.SetBGMVolume(value);
                break;
            case VolumeType.SFX:
                SettingsManager.Instance.SetSFXVolume(value);
                break;
        }
    }
}
