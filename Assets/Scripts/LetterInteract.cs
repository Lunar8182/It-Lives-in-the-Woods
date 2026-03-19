using UnityEngine;
using TMPro;

public class LetterInteract : MonoBehaviour
{
    public PlayerMovement playerController;
    [TextArea(5, 10)]
    public string letterText;

    public GameObject letterUIPanel;
    public TextMeshProUGUI letterUIText;

    private bool isReading = false;

    public void InteractPaper()
    {
        if (!isReading)
        {
            OpenLetter();
        }
        else
        {
            CloseLetter();
        }
    }

    void OpenLetter()
    {
        letterUIPanel.SetActive(true);
        letterUIText.text = letterText;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        isReading = true;
    }

    void CloseLetter()
    {
        letterUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isReading = false;
    }
}