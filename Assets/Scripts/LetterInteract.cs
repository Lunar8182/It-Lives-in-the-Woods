using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class LetterInteract : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject letterUIPanel;
    public TextMeshProUGUI letterUIText;
    public GameObject gameHUD;
    public GameObject pressEButton;
    public GameObject pressESCMsg;

    [Header("Visuals")]
    public Volume postProcessVolume;

    [Header("Player & Camera Objects")]
    public PlayerMovement playerController;
    public GameObject cameraObject;

    [Header("Content")]
    [TextArea(10, 15)] public string letterText;

    public bool isReading {get; private set;}

    void Update()
    {
        if (isReading && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLetter();
        }
    }

    public void InteractPaper()
    {
        if (isReading) return;
        OpenLetter();
    }

    void OpenLetter()
    {
        isReading = true;

        letterUIPanel.SetActive(true);
        letterUIText.text = letterText;
        pressESCMsg.SetActive(true);

        if (gameHUD != null) gameHUD.SetActive(false);
        if (pressEButton != null) pressEButton.SetActive(false);

        if (playerController != null) playerController.enabled = false;

        if (cameraObject != null)
        {
            MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts)
            {
                s.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        if (postProcessVolume != null) postProcessVolume.weight = 1f;
    }

    public void CloseLetter()
    {
        isReading = false;

        letterUIPanel.SetActive(false);
        pressESCMsg.SetActive(false);

        if (gameHUD != null) gameHUD.SetActive(true);

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

        if (postProcessVolume != null) postProcessVolume.weight = 0f;
    }
}














// using UnityEngine;
// using TMPro;

// public class LetterInteract : MonoBehaviour
// {
//     public PlayerMovement playerController;
//     [TextArea(5, 10)]
//     public string letterText;

//     public GameObject letterUIPanel;
//     public TextMeshProUGUI letterUIText;
//     private bool isReading = false;

//     public void InteractPaper()
//     {
//         if (!isReading)
//         {
//             OpenLetter();
//         }
//         else
//         {
//             CloseLetter();
//         }
//     }

//     void OpenLetter()
//     {
//         letterUIPanel.SetActive(true);
//         letterUIText.text = letterText;

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         if (playerController != null)
//         {
//             playerController.enabled = false;
//         }

//         isReading = true;
//     }

//     void CloseLetter()
//     {
//         letterUIPanel.SetActive(false);

//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         if (playerController != null)
//         {
//             playerController.enabled = true;
//         }

//         isReading = false;
//     }
// }