using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PotEndingInteract : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource fireAudioSource; 
    public AudioSource interactionAudioSource; 
    public AudioClip burnDollSound;

    [Header("UI Elements")]
    public GameObject pressEText; 
    public GameObject missingDollMessage; 

    [Header("Ending Setup")]
    public string endingSceneName = "GoodEnding";

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

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
        if (interactionAudioSource != null && burnDollSound != null)
        {
            interactionAudioSource.PlayOneShot(burnDollSound);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(endingSceneName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            
            if (fireAudioSource != null) fireAudioSource.Play();
            if (pressEText != null) pressEText.SetActive(true); 

            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            

            if (fireAudioSource != null) fireAudioSource.Stop();
            if (pressEText != null) pressEText.SetActive(false); 
        }
    }

    IEnumerator ShowMissingDollText()
    {
        if (pressEText != null) pressEText.SetActive(false);
        if (missingDollMessage != null) missingDollMessage.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (missingDollMessage != null) missingDollMessage.SetActive(false);
        
        if (playerNearby && pressEText != null) pressEText.SetActive(true);
    }
}