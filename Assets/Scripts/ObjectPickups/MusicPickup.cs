using UnityEngine;

public class MusicPickup : MonoBehaviour
{
    public GameObject playerMusic;
    public GameObject pressEText;

    private bool playerNearby = false;
    private AudioSource audioSource;
    private bool isHeld = false; // Tracks if the radio is in your hand

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 1. If it's on the ground and player is nearby, press E to pick up
        if (!isHeld && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpRadio();
        }

        // 2. If it's already held, Left Click to toggle the music
        if (isHeld && Input.GetMouseButtonDown(0))
        {
            ToggleMusic();
        }
    }

    void PickUpRadio()
    {
        isHeld = true;
        playerMusic.SetActive(true);
        pressEText.SetActive(false);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    void ToggleMusic()
    {
        AudioSource playerSource = playerMusic.GetComponent<AudioSource>();

        if (playerSource.isPlaying)
        {
            playerSource.Pause();
        }
        else
        {
            playerSource.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isHeld)
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