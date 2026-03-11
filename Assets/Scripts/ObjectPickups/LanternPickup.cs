using UnityEngine;

public class LanternPickup : MonoBehaviour
{
    public GameObject playerLantern;


    public GameObject keyPrompt;
    private bool playerNearby = false;
    private AudioSource audioSource;
    private bool voicePlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpLantern();
        }
    }

    void PickUpLantern()
    {
        playerLantern.SetActive(true);
        keyPrompt.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            keyPrompt.SetActive(true);

            if (!voicePlayed)
            {
                audioSource.Play();
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