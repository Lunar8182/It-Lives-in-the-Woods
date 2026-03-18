using UnityEngine;
using TMPro;

public class LetterInteract : MonoBehaviour
{
    [TextArea(5, 10)]
    public string letterText;

    public GameObject letterUIPanel;
    public TextMeshProUGUI letterUIText;

    private bool isReading = false;

    public void Interact()
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

        isReading = true;
    }

    void CloseLetter()
    {
        letterUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isReading = false;
    }
}