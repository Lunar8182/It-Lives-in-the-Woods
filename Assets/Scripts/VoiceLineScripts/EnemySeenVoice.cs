using UnityEngine;
using System.Collections;

public class EnemySeenVoice : MonoBehaviour
{
    public AudioClip scarySound;
    public AudioClip voiceLine;
    public AudioSource playerAudio;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(PlaySounds());
        }
    }

    IEnumerator PlaySounds()
    {
        playerAudio.PlayOneShot(scarySound);

        yield return new WaitForSeconds(2f);

        playerAudio.PlayOneShot(voiceLine, 3f);
    }
}