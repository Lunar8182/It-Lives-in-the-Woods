using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI; // --- NEW: Added the Settings Panel ---

    [HideInInspector]
    public bool isPaused = false;
    public string titleSceneName = "TitleScreen";

    void Update()
    {
        // Check if the player presses the P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // --- UPDATED: Force BOTH menus to close when unpausing ---
        pauseMenuUI.SetActive(false);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false);

        Time.timeScale = 1f; // Unfreeze time
        isPaused = false;

        // Lock the cursor back to the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        // Only turn on the main pause menu when we first pause
        pauseMenuUI.SetActive(true);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false);

        Time.timeScale = 0f; // Freeze time
        isPaused = true;

        // Unlock the cursor so the player can click menu buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- NEW: Replaced the MenuManager script with these two functions ---
    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void GoToTitleScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}