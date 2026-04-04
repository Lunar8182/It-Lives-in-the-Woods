using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator anim;
    public Transform[] patrolPoints;
    public GameObject gameOverScreen;
    public GameObject mainCanvas;

    [Header("Cameras")]
    public Camera playerCamera;
    public Camera jumpscareCamera;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpscareSound;
    public AudioClip stunnedSound;
    public AudioClip afterStunSound;

    [Header("Movement")]
    public float detectionRange = 50f;
    public float fieldOfView = 90f;
    public float chaseSpeed = 12f;
    public float roamSpeed = 3f;
    public float patrolWaitTime = 3f;
    public float searchDuration = 10f;
    public float loseSightDelay = 2f;
    public float searchRadius = 15f;

    [Header("Stun Settings")]
    public float stunDuration = 5f;

    [Header("Jumpscare")]
    public float jumpscareDistance = 3f;
    public float jumpscareDuration = 2f;
    public float shakeIntensity = 0.3f;
    public float shakeSpeed = 20f;

    [Header("Lighting")]
    public Light enemyLight;
    public Color roamColor = Color.green;
    public Color searchColor = Color.yellow;
    public Color chaseColor = Color.red;

    public Vector3 jumpscareLightOffset = new Vector3(0f, -1f, -0.5f);
    private Vector3 originalLightPosition;

    private NavMeshAgent agent;
    private PlayerCam playerCamScript;
    private CharacterController playerController;

    private Vector3 lastSeenPosition;
    private Vector3 initialJumpscareCamLocalPos;

    private float searchTimer;
    private float loseSightTimer;
    private float patrolTimer;
    private float jumpscareTimer;
    private float stunTimer;

    private int lastPatrolIndex = -1;
    private bool isJumpscaring = false;
    private bool isGameOver = false;

    private bool reachedLastSeen = false;
    private float pointWaitTimer = 0f;

    private int animStateHash = Animator.StringToHash("State");

    public enum EnemyState { Roaming, Chasing, Searching, Jumpscare, Stunned }
    public EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCamScript = player.GetComponentInChildren<PlayerCam>();
        playerController = player.GetComponent<CharacterController>();
        enemyLight.enabled = true;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (jumpscareCamera != null)
        {
            initialJumpscareCamLocalPos = jumpscareCamera.transform.localPosition;
            jumpscareCamera.gameObject.SetActive(false);
        }

        if (enemyLight != null)
        {
            originalLightPosition = enemyLight.transform.localPosition;
        }

        patrolTimer = patrolWaitTime;
        currentState = EnemyState.Roaming;
        PickRoamPoint();
    }

    void Update()
    {
        if (isGameOver) return;

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.T))
        {
            StunEnemy();
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);

        bool playerVisible = CanSeePlayer();

        if (!isJumpscaring && currentState != EnemyState.Stunned && Vector3.Distance(transform.position, player.position) <= jumpscareDistance)
            StartJumpscare();

        switch (currentState)
        {
            case EnemyState.Roaming:
                if (enemyLight != null) enemyLight.color = roamColor;
                anim.SetInteger(animStateHash, 0);
                Roam();
                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                if (enemyLight != null) enemyLight.color = chaseColor;
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
                        reachedLastSeen = false;
                    }
                }
                break;

            case EnemyState.Searching:
                if (enemyLight != null) enemyLight.color = searchColor;
                anim.SetInteger(animStateHash, 2);
                Search();
                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Stunned:
                agent.isStopped = true;

                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                    agent.isStopped = false;

                    if (afterStunSound != null && audioSource != null && !audioSource.isPlaying)
                    {
                        audioSource.clip = afterStunSound;
                        audioSource.Play();
                    }

                    currentState = EnemyState.Searching;
                    searchTimer = searchDuration;
                    reachedLastSeen = false;
                    lastSeenPosition = transform.position;
                }
                break;

            case EnemyState.Jumpscare:
                if (enemyLight != null)
                {
                    enemyLight.color = Color.red;
                    enemyLight.intensity = Random.Range(10f, 100f);
                    enemyLight.range = 1000000f;
                }
                jumpscareTimer -= Time.deltaTime;
                if (jumpscareTimer <= 0f)
                    EndJumpscare();
                break;
        }
    }

    public void StunEnemy()
    {
        if (currentState == EnemyState.Jumpscare || isGameOver) return;

        if (stunnedSound != null && audioSource != null)
        {
            audioSource.clip = stunnedSound;
            audioSource.Play();
        }

        currentState = EnemyState.Stunned;
        stunTimer = stunDuration;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        anim.SetInteger(animStateHash, 5);
    }

    void StartJumpscare()
    {
        if (isJumpscaring) return;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        isJumpscaring = true;
        currentState = EnemyState.Jumpscare;
        agent.isStopped = true;

        anim.SetInteger(animStateHash, 3);
        anim.SetTrigger("Jumpscare");
        jumpscareTimer = jumpscareDuration;

        if (enemyLight != null)
        {
            enemyLight.enabled = true;
            enemyLight.color = chaseColor;
            enemyLight.transform.localPosition = originalLightPosition + jumpscareLightOffset;
        }

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

        if (mainCanvas != null) mainCanvas.SetActive(false);

        playerCamera.gameObject.SetActive(false);
        jumpscareCamera.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (!isJumpscaring || jumpscareCamera == null) return;

        float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
        float y = (Mathf.PerlinNoise(100f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

        jumpscareCamera.transform.localPosition = initialJumpscareCamLocalPos + new Vector3(x, y, 0);
    }

    void EndJumpscare()
    {
        isJumpscaring = false;
        isGameOver = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainCanvas != null) mainCanvas.SetActive(true);
        gameOverScreen.SetActive(true);
    }

    void Roam()
    {
        agent.isStopped = false;
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
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        if (Vector3.Distance(agent.destination, player.position) > 1f)
            agent.SetDestination(player.position);
    }

    void Search()
    {
        agent.isStopped = false;
        agent.speed = roamSpeed;
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0f)
        {
            currentState = EnemyState.Roaming;
            reachedLastSeen = false;
            PickRoamPoint();
            return;
        }

        if (!reachedLastSeen)
        {
            if (Vector3.Distance(agent.destination, lastSeenPosition) > 1f)
                agent.SetDestination(lastSeenPosition);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                reachedLastSeen = true;
                pointWaitTimer = 1.5f;
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                if (pointWaitTimer > 0)
                {
                    pointWaitTimer -= Time.deltaTime;
                    LookAround();
                }
                else
                {
                    Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
                    randomDirection += lastSeenPosition;

                    if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, searchRadius, 1))
                    {
                        agent.SetDestination(hit.position);
                        pointWaitTimer = 1.5f;
                    }
                }
            }
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
        agent.SetDestination(patrolPoints[chosenIndex].position);
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

    void LookAround()
    {
        anim.SetInteger(animStateHash, 4);
        transform.Rotate(0, 40f * Time.deltaTime, 0);
    }

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