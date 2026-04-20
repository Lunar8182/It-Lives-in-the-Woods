using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("References")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("SavedMasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            volumeSlider.value = 0.5f;
            SetVolume();
        }
    }

    public void SetVolume()
    {
        float volume = volumeSlider.value;

        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("SavedMasterVolume", volume);
    }

    private void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("SavedMasterVolume");

        SetVolume();
    }
}