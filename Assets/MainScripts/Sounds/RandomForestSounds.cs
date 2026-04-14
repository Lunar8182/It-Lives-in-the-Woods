using UnityEngine;

public class RandomForestSounds : MonoBehaviour
{
    public AudioClip[] randomClips;
    public float minDelay = 10f;
    public float maxDelay = 25f;

    private AudioSource audioSource;
    private float timer;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetNewTimer();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayRandomSound();
            SetNewTimer();
        }
    }

    void PlayRandomSound()
    {
        if (randomClips.Length > 0)
        {
            AudioClip clip = randomClips[Random.Range(0, randomClips.Length)];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }

    void SetNewTimer()
    {
        timer = Random.Range(minDelay, maxDelay);
    }
}
