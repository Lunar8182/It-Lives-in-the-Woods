using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PotEndingInteract : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource fireAudioSource;

    [Header("UI Elements")]
    public GameObject pressEText;
    public GameObject missingDollMessage;

    [Header("Ending Setup")]
    public string endingSceneName = "GoodEnding";

    public void Interact()
    {
        if (InventoryManager.instance.hasVoodooDoll)
        {
            TriggerEndingSequence();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowMissingDollText());
        }
    }

    void TriggerEndingSequence()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(endingSceneName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fireAudioSource != null) fireAudioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fireAudioSource != null) fireAudioSource.Stop();
        }
    }

    IEnumerator ShowMissingDollText()
    {
        if (pressEText != null) pressEText.SetActive(false);
        if (missingDollMessage != null) missingDollMessage.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (missingDollMessage != null) missingDollMessage.SetActive(false);
    }
}