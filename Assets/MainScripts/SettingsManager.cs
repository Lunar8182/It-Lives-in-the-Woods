using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sensitivitySlider;
    public TMP_Dropdown fpsDropdown;

    void Start()
    {

        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSens;
        }


        int savedFPS = PlayerPrefs.GetInt("FPSLimit", 1);

        if (fpsDropdown != null)
        {
            fpsDropdown.value = savedFPS;
        }

        ApplyFPS(savedFPS);
    }

    public void SetSensitivity(float newSensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", newSensitivity);
        PlayerPrefs.Save();
    }

    public void SetFPS(int fpsIndex)
    {
        PlayerPrefs.SetInt("FPSLimit", fpsIndex);
        PlayerPrefs.Save();

        ApplyFPS(fpsIndex);
    }

    private void ApplyFPS(int index)
    {
        QualitySettings.vSyncCount = 0;

        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = -1; break;
        }
    }
}