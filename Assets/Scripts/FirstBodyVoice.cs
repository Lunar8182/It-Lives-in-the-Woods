using UnityEngine;

public class FirstBodyVoice : MonoBehaviour
{

    public AudioClip voiceLine;
    private AudioSource audioSource;

    private bool voicePlayed = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!voicePlayed)
            {
                audioSource.PlayOneShot(voiceLine);
                voicePlayed = true;
            }
        }
    }


}
