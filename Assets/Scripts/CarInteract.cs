using UnityEngine;
using System.Collections;

public class CarInteract : MonoBehaviour
{
    public GameObject keyPrompt;
    public AudioClip voiceLine1;
    public AudioClip voiceLine2;

    public GameObject pressEText;

    private bool playerNearby = false;
    private bool voice1Played = false;
    private bool voice2Played = false;

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
        else if (!voice2Played)
        {
            voice.PlayOneShot(voiceLine2);
            voice2Played = true;
        }
        else
        {
            StartCoroutine(ShowTextTemporary());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            //pressEText.SetActive(true);
            keyPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            //pressEText.SetActive(false);
            keyPrompt.SetActive(false);
        }
    }


    IEnumerator ShowTextTemporary()
    {
        pressEText.SetActive(true);

        yield return new WaitForSeconds(3f);

        pressEText.SetActive(false);
    }
}