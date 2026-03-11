using UnityEngine;

public class Interactable : MonoBehaviour
{
    //This script is the default for picking up any item in the game.
    public GameObject objectToActivate; // item player receives
    public GameObject keyPrompt;

    public AudioClip voiceLine;

    private bool playerNearby = false;
    private bool voicePlayed = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        keyPrompt.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            keyPrompt.SetActive(true);

            if (audioSource && voiceLine && !voicePlayed)
            {
                audioSource.PlayOneShot(voiceLine);
                voicePlayed = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            keyPrompt.SetActive(false);
        }
    }
}