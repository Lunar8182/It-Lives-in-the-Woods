using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;

    [Header("Journal UI")]
    public GameObject journalUIPanel;
    public TextMeshProUGUI journalReadingText;

    [Header("Letter 1 Button")]
    public Button letter1Button;
    public TextMeshProUGUI letter1ButtonText;

    [Header("Letter 2 Button")]
    public Button letter2Button;
    public TextMeshProUGUI letter2ButtonText;

    [Header("Player Controllers (To disable when open)")]
    public PlayerMovement playerController;
    public GameObject cameraObject; // <-- CHANGED THIS TO GAMEOBJECT

    // Hidden variables to store our data
    private bool isJournalOpen = false;
    private bool hasLetter1 = false;
    private bool hasLetter2 = false;
    private string textForLetter1 = "";
    private string textForLetter2 = "";

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        journalUIPanel.SetActive(false);

        letter1ButtonText.text = "???";
        letter1Button.interactable = false;

        letter2ButtonText.text = "???";
        letter2Button.interactable = false;

        journalReadingText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isJournalOpen) CloseJournal();
            else OpenJournal();
        }
    }

    public void UnlockLetterInJournal(int letterID, string letterContent)
    {
        if (letterID == 1)
        {
            hasLetter1 = true;
            textForLetter1 = letterContent;
            letter1ButtonText.text = "Letter 1";
            letter1Button.interactable = true;
        }
        else if (letterID == 2)
        {
            hasLetter2 = true;
            textForLetter2 = letterContent;
            letter2ButtonText.text = "Letter 2";
            letter2Button.interactable = true;
        }
    }

    void OpenJournal()
    {
        isJournalOpen = true;
        journalUIPanel.SetActive(true);
        if (journalReadingText.text == "") journalReadingText.text = "Select a letter to read...";

        if (playerController != null) playerController.enabled = false;

        // --- NEW CAMERA FREEZE LOGIC ---
        if (cameraObject != null)
        {
            MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts)
            {
                s.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseJournal()
    {
        isJournalOpen = false;
        journalUIPanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;

        // --- NEW CAMERA UNFREEZE LOGIC ---
        if (cameraObject != null)
        {
            MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts)
            {
                s.enabled = true;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReadLetter1()
    {
        if (hasLetter1) journalReadingText.text = textForLetter1;
    }

    public void ReadLetter2()
    {
        if (hasLetter2) journalReadingText.text = textForLetter2;
    }
}