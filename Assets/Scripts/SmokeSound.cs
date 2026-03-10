using UnityEngine;

public class SmokeSound : MonoBehaviour
{

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();


        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Stop();
        }
    }
}

