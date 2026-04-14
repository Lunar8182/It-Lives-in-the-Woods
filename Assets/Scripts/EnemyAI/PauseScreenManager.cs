using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenManager : MonoBehaviour
{
    public string titleSceneName = "TitleScreen";

    public void GoToTitleScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}