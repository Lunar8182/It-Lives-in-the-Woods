using UnityEngine;

public class StartupSettings : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;
    }
}