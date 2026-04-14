using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InDepthInteract : MonoBehaviour
{

    public GameObject keyPrompt;
    public AudioClip voiceLine1;
    public AudioClip voiceLine2;

    public GameObject pressETextCar;

    [Header("Alternate Ending")]
    public string alternateEndingSceneName = "AlternateEnding";



    void Start()
    {
    }

    public void Interact()
    {

        if (InventoryManager.instance.hasWrench)
        {
            SceneManager.LoadScene(alternateEndingSceneName);
            return;
        }
        else
        {
            StartCoroutine(ShowTextTemporaryCar());
        }
    }

    IEnumerator ShowTextTemporaryCar()
    {
        pressETextCar.SetActive(true);
        yield return new WaitForSeconds(3f);
        pressETextCar.SetActive(false);
    }
}