using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator anim;
    public Transform[] patrolPoints;
    public GameObject gameOverScreen;
    
    [Header("Cameras")]
    public Camera playerCamera;
    public Camera jumpscareCamera;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpscareSound;

    [Header("Movement")]
    public float roamRadius = 100f;
    public float detectionRange = 50f;
    public float fieldOfView = 90f;
    public float chaseSpeed = 12f;
    public float roamSpeed = 3f;
    public float patrolWaitTime = 3f;
    public float searchDuration = 5f;
    public float loseSightDelay = 2f;

    [Header("Jumpscare")]
    public float jumpscareDistance = 3f;
    public float jumpscareDuration = 2f;
    public float shakeIntensity = 0.3f;
    public float shakeSpeed = 20f;

    [Header("Jumpscare Camera Offsets")]
    public Vector3 cameraPositionOffset = new Vector3(0f, 1.6f, 0f); // relative to player
    public Vector3 cameraLookOffset = new Vector3(0f, 1.5f, 0f);     // relative to enemy

    private NavMeshAgent agent;
    private PlayerCam playerCamScript;
    private CharacterController playerController;

    private Vector3 lastSeenPosition;
    private Vector3 roamPoint;
    private Vector3 initialJumpscareCamLocalPos;

    private float searchTimer;
    private float loseSightTimer;
    private float patrolTimer;
    private float jumpscareTimer;

    private int lastPatrolIndex = -1;
    private bool isJumpscaring = false;
    private bool hasSnapped = false;

    private int animStateHash = Animator.StringToHash("State");

    private enum EnemyState { Roaming, Chasing, Searching, Jumpscare }
    private EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCamScript = player.GetComponentInChildren<PlayerCam>();
        playerController = player.GetComponent<CharacterController>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (jumpscareCamera != null)
        {
            initialJumpscareCamLocalPos = jumpscareCamera.transform.localPosition;
            jumpscareCamera.gameObject.SetActive(false); // Make sure it's off to start
        }

        patrolTimer = patrolWaitTime;
        currentState = EnemyState.Roaming;
        PickRoamPoint();
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        bool playerVisible = CanSeePlayer();

        if (!isJumpscaring && Vector3.Distance(transform.position, player.position) <= jumpscareDistance)
            StartJumpscare();

        switch (currentState)
        {
            case EnemyState.Roaming:
                anim.SetInteger(animStateHash, 0);
                Roam();
                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                anim.SetInteger(animStateHash, 1);
                Chase();
                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    loseSightTimer = loseSightDelay;
                }
                else
                {
                    loseSightTimer -= Time.deltaTime;
                    if (loseSightTimer <= 0)
                    {
                        currentState = EnemyState.Searching;
                        searchTimer = searchDuration;
                    }
                }
                break;

            case EnemyState.Searching:
                anim.SetInteger(animStateHash, 2);
                Search();
                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Jumpscare:
                jumpscareTimer -= Time.deltaTime;
                if (jumpscareTimer <= 0f)
                    EndJumpscare();
                break;
        }
    }

    void StartJumpscare()
    {
        if (isJumpscaring) return;

        isJumpscaring = true;
        currentState = EnemyState.Jumpscare;
        agent.isStopped = true;
        anim.SetTrigger("Jumpscare");
        jumpscareTimer = jumpscareDuration;

        // Force enemy to face player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (jumpscareSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = jumpscareSound;
            audioSource.Play();
        }

        if (playerCamScript != null) playerCamScript.enabled = false;
        if (playerController != null) playerController.enabled = false;

        // THE MAGIC: Turn off the player's eyes, turn on the enemy's camera
        playerCamera.gameObject.SetActive(false);
        jumpscareCamera.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (!isJumpscaring || jumpscareCamera == null) return;

        // Apply a violent shake directly to the dedicated camera
        float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
        float y = (Mathf.PerlinNoise(100f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

        // Shake relative to its local position so it stays locked onto the enemy's face
        jumpscareCamera.transform.localPosition = initialJumpscareCamLocalPos + new Vector3(x, y, 0);
    }

    void EndJumpscare()
    {
        isJumpscaring = false;

        // Game over
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverScreen.SetActive(true);
    }

    void Roam()
    {
        agent.speed = roamSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.3f)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0f)
            {
                PickRoamPoint();
                patrolTimer = patrolWaitTime;
            }
            else LookAround();
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        if (Vector3.Distance(agent.destination, player.position) > 1f)
            agent.SetDestination(player.position);
    }

    void Search()
    {
        agent.speed = roamSpeed;
        if (agent.destination != lastSeenPosition)
            agent.SetDestination(lastSeenPosition);

        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            searchTimer = searchDuration;
            currentState = EnemyState.Roaming;
            PickRoamPoint();
        }
    }

    void PickRoamPoint()
    {
        if (patrolPoints.Length == 0) return;

        int chosenIndex = -1;
        float bestScore = float.MinValue;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (i == lastPatrolIndex) continue;

            float distanceToPlayer = Vector3.Distance(patrolPoints[i].position, player.position);
            float score = Random.value * 20f - distanceToPlayer;
            if (score > bestScore)
            {
                bestScore = score;
                chosenIndex = i;
            }
        }

        if (chosenIndex == -1) return;
        lastPatrolIndex = chosenIndex;
        roamPoint = patrolPoints[chosenIndex].position;
        agent.SetDestination(roamPoint);
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, detectionRange))
            return hit.transform.root == player;

        return false;
    }

    void LookAround() => transform.Rotate(0, 40f * Time.deltaTime, 0);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, left * detectionRange);
        Gizmos.DrawRay(transform.position, right * detectionRange);
    }
}