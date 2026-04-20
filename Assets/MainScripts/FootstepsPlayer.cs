using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepSounds; 
    
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
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }
}