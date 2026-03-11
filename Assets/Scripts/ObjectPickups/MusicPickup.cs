using UnityEngine;

public class MusicPickup : MonoBehaviour
{
    public GameObject playerMusic;
    public GameObject keyPrompt;
    private bool playerNearby = false;
    private AudioSource audioSource;
    private bool isHeld = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isHeld && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpRadio();
        }

        if (isHeld && Input.GetMouseButtonDown(0))
        {
            ToggleMusic();
        }
    }

    void PickUpRadio()
    {
        isHeld = true;
        playerMusic.SetActive(true);
        keyPrompt.SetActive(false);

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
            keyPrompt.SetActive(true);
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