using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public string gameSceneName = "MainGame";
    public GameObject titleScreen;
    public GameObject settingsScreen;

    public void StartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (titleScreen != null) titleScreen.SetActive(false);
        if (settingsScreen != null) settingsScreen.SetActive(true);
    }

    public void CloseSettings()
    {
        if (titleScreen != null) titleScreen.SetActive(true);
        if (settingsScreen != null) settingsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
