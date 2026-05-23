using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndingCutsceneManager : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadTitleScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Credits");
    }
}