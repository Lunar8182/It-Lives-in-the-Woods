using UnityEngine;

public class FootstepsEnemy : MonoBehaviour
{
    [Header("Link to Main AI")]
    public EnemyAI aiScript;

    [Header("Audio Setup")]
    public AudioSource footstepSource; 
    public AudioClip[] footstepSounds; 

    [Header("Pacing")]
    public float roamStepInterval = 0.6f;   
    public float searchStepInterval = 0.5f; 
    public float chaseStepInterval = 0.25f; 

    [Header("Creepy Settings")]
    public bool disorientPlayer = true;

    private float stepTimer;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (aiScript.currentState == EnemyAI.EnemyState.Jumpscare) return;

        float currentInterval = roamStepInterval; 
        
        if (aiScript.currentState == EnemyAI.EnemyState.Searching)
        {
            currentInterval = searchStepInterval;
        }
        else if (aiScript.currentState == EnemyAI.EnemyState.Chasing)
        {
            currentInterval = chaseStepInterval;
        }

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float currentSpeed = distanceMoved / Time.deltaTime;

        if (currentSpeed > 0.1f)
        {
            stepTimer += Time.deltaTime; 

            if (stepTimer >= currentInterval)
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
            
            if (disorientPlayer)
            {
                footstepSource.pitch = Random.Range(0.6f, 1.4f);
                footstepSource.panStereo = Random.Range(-0.8f, 0.8f); 
            }
            else
            {
                footstepSource.pitch = Random.Range(0.9f, 1.1f);
                footstepSource.panStereo = 0f; 
            }
            
            if (aiScript.currentState == EnemyAI.EnemyState.Chasing)
            {
                footstepSource.volume = 1.0f;
            }
            else
            {
                footstepSource.volume = 0.7f; 
            }

            footstepSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }
}