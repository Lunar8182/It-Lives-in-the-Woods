using UnityEngine;
using System.Collections;

public class PotInteract : MonoBehaviour
{
    public AudioSource bubbleSource;
    public AudioSource voiceSource;

    public AudioClip voiceline;
    public AudioClip voiceLine2;

    public GameObject keyPrompt;

    public GameObject pressEText;

    private bool playerNearby = false;
    private bool voicePlayed = false;
    private bool voice2Played = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (voiceSource.isPlaying) return;

        if (!voicePlayed)
        {
            voiceSource.PlayOneShot(voiceline);
            voicePlayed = true;
        }
        else if (!voice2Played)
        {
            voiceSource.PlayOneShot(voiceLine2);
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
            keyPrompt.SetActive(true);

            bubbleSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            keyPrompt.SetActive(false);
            bubbleSource.Stop();
        }
    }

    IEnumerator ShowTextTemporary()
    {
        pressEText.SetActive(true);

        yield return new WaitForSeconds(3f);

        pressEText.SetActive(false);
    }
}