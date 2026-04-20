using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Footstep Sounds by Material")]
    public AudioClip[] defaultSounds;
    public AudioClip[] grassSounds;
    public AudioClip[] woodSounds;
    public AudioClip[] metalSounds;

    [Header("Pacing")]
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    private float currentStepInterval;
    private float stepTimer;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        currentStepInterval = walkStepInterval;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentStepInterval = sprintStepInterval;
        }
        else
        {
            currentStepInterval = walkStepInterval;
        }

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float currentSpeed = distanceMoved / Time.deltaTime;

        if (currentSpeed > 0.1f)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= currentStepInterval)
            {
                PlayStep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void PlayStep()
    {
        AudioClip[] arrayToPlay = defaultSounds;


        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            switch (hit.collider.tag)
            {
                case "Wood":
                    arrayToPlay = woodSounds;
                    break;
                case "Metal":
                    arrayToPlay = metalSounds;
                    break;
                case "Grass":
                    arrayToPlay = grassSounds;
                    break;
            }
        }

        if (arrayToPlay.Length > 0)
        {
            int randomIndex = Random.Range(0, arrayToPlay.Length);

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(arrayToPlay[randomIndex]);
        }
    }
}