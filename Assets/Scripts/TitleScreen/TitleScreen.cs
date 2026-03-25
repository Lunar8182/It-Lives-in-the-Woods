using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public string gameSceneName = "MainGame";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(gameSceneName);
    }

    // Update is called once per frame
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
