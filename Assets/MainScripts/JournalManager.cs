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
    [Header("Letter 3 Button")]
    public Button letter3Button;
    public TextMeshProUGUI letter3ButtonText;
    [Header("Letter 4 Button")]
    public Button letter4Button;
    public TextMeshProUGUI letter4ButtonText;
    [Header("Letter 5 Button")]
    public Button letter5Button;
    public TextMeshProUGUI letter5ButtonText;
    [Header("Letter 6 Button")]
    public Button letter6Button;
    public TextMeshProUGUI letter6ButtonText;
    [Header("Letter 7 Button")]
    public Button letter7Button;
    public TextMeshProUGUI letter7ButtonText;

    [Header("Player Controllers (To disable when open)")]
    public PlayerMovement playerController;
    public GameObject cameraObject;

    private bool isJournalOpen = false;
    private bool hasLetter1 = false;
    private bool hasLetter2 = false;
    private bool hasLetter3 = false;
    private bool hasLetter4 = false;
    private bool hasLetter5 = false;
    private bool hasLetter6 = false;
    private bool hasLetter7 = false;
    private string textForLetter1 = "";
    private string textForLetter2 = "";
    private string textForLetter3 = "";
    private string textForLetter4 = "";
    private string textForLetter5 = "";
    private string textForLetter6 = "";
    private string textForLetter7 = "";


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
        letter3ButtonText.text = "???";
        letter3Button.interactable = false;
        letter4ButtonText.text = "???";
        letter4Button.interactable = false;
        letter5ButtonText.text = "???";
        letter5Button.interactable = false;
        letter6ButtonText.text = "???";
        letter6Button.interactable = false;
        letter7ButtonText.text = "???";
        letter7Button.interactable = false;

        journalReadingText.text = "";
    }

    void Update()
    {
        if (isJournalOpen && Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.J))
        {
            CloseJournal();
        }
        if (!isJournalOpen && Input.GetKeyDown(KeyCode.J))
        {
            OpenJournal();
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
        else if (letterID == 3)
        {
            hasLetter3 = true;
            textForLetter3 = letterContent;
            letter3ButtonText.text = "Letter 3";
            letter3Button.interactable = true;
        }
        else if (letterID == 4)
        {
            hasLetter4 = true;
            textForLetter4 = letterContent;
            letter4ButtonText.text = "Letter 4";
            letter4Button.interactable = true;
        }
        else if (letterID == 5)
        {
            hasLetter5 = true;
            textForLetter5 = letterContent;
            letter5ButtonText.text = "Letter 5";
            letter5Button.interactable = true;
        }
        else if (letterID == 6)
        {
            hasLetter6 = true;
            textForLetter6 = letterContent;
            letter6ButtonText.text = "Letter 6";
            letter6Button.interactable = true;
        }
        else if (letterID == 7)
        {
            hasLetter7 = true;
            textForLetter7 = letterContent;
            letter7ButtonText.text = "Letter 7";
            letter7Button.interactable = true;
        }
    }

    void OpenJournal()
    {
        isJournalOpen = true;
        journalUIPanel.SetActive(true);
        if (journalReadingText.text == "") journalReadingText.text = "Select a letter to read...";

        if (playerController != null)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerController.enabled = false;
        }
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
    public void ReadLetter3()
    {
        if (hasLetter3) journalReadingText.text = textForLetter3;
    }
    public void ReadLetter4()
    {
        if (hasLetter4) journalReadingText.text = textForLetter4;
    }
    public void ReadLetter5()
    {
        if (hasLetter5) journalReadingText.text = textForLetter5;
    }
    public void ReadLetter6()
    {
        if (hasLetter6) journalReadingText.text = textForLetter6;
    }
    public void ReadLetter7()
    {
        if (hasLetter7) journalReadingText.text = textForLetter7;
    }
}