using UnityEngine;
using System.Collections;

public class BubblingSound : MonoBehaviour
{
    private AudioSource voiceline1;

    public AudioClip voiceline;
    public AudioClip voiceLine2;

    public GameObject eText;        // "Press E" prompt
    public GameObject pressEText;   // message after both lines

    private bool playerNearby = false;
    private bool voicePlayed = false;
    private bool voice2Played = false;

    void Start()
    {
        voiceline1 = GetComponent<AudioSource>();
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
        if (voiceline1.isPlaying) return;

        if (!voicePlayed)
        {
            voiceline1.PlayOneShot(voiceline);
            voicePlayed = true;
        }
        else if (!voice2Played)
        {
            voiceline1.PlayOneShot(voiceLine2);
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
            eText.SetActive(true);
        }
        else if (other.CompareTag("Player") && voicePlayed && voice2Played)
        {
            playerNearby = true;
            eText.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            eText.SetActive(false);
        }
    }

    IEnumerator ShowTextTemporary()
    {
        pressEText.SetActive(true);

        yield return new WaitForSeconds(3f);

        pressEText.SetActive(false);
    }
}