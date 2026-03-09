using UnityEngine;

public class LanternPickup : MonoBehaviour
{
    public GameObject playerLantern;
    public GameObject pressEText;

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
        pressEText.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            pressEText.SetActive(true);

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
            pressEText.SetActive(false);
        }
    }
}