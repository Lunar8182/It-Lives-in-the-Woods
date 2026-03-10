using UnityEngine;

public class CarInteract : MonoBehaviour
{
    public GameObject pressEText;

    public AudioClip voiceLine1;
    public AudioClip voiceLine2;

    private bool playerNearby = false;
    private bool voice1Played = false;

    private AudioSource voice;

    void Start()
    {
        voice = GetComponent<AudioSource>();
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
        if (voice.isPlaying) return;

        if (!voice1Played)
        {
            voice.PlayOneShot(voiceLine1);
            voice1Played = true;
        }
        else
        {
            voice.PlayOneShot(voiceLine2);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            pressEText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            pressEText.SetActive(false);
        }
    }
}