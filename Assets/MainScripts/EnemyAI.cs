using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator anim;
    public Transform[] patrolPoints;
    public GameObject gameOverScreen;
    public GameObject mainCanvas;
    public GameObject forestAmbience;
    public GameObject MapIcon;

    [Header("Cameras")]
    public Camera playerCamera;
    public Camera jumpscareCamera;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpscareSound;
    public AudioClip stunnedSound;
    public AudioClip afterStunSound;
    public AudioClip shuttingCell;

    [Header("State Audio")]
    public AudioClip[] roamSounds;
    public AudioClip[] chaseSounds;
    public AudioClip[] searchSounds;
    public float minSoundInterval = 4f;
    public float maxSoundInterval = 10f;

    [Header("Enraged Settings")]
    public AudioClip enragedRoarSound;
    public float enragedSpeed = 15f;
    public float enragedWaitTime = 0.2f;
    private bool isEnraged = false;
    public float enragedIntensityMult = 2f;
    public float enragedRangeMult = 1.5f;

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

    [Header("Capture Setup")]
    public bool enableCaptureFeature = true;
    public GameObject blackoutPanel;
    public Transform playerPrisonSpawn;
    public Transform enemyPostCaptureSpawn;
    public Transform enemyWalkAwayPoint;
    public float blackoutDuration = 3f;
    public bool hasBeenCaptured = false;
    public float stareDuration = 5f;
    public float fadeInSpeed = 0.5f;

    [Header("Capture Audio")]
    public AudioClip captureVoiceline;
    public AudioClip staringVoiceline;

    [Header("Lighting")]
    public Light enemyLight;
    public Color roamColor = Color.green;
    public Color searchColor = Color.yellow;
    public Color chaseColor = Color.red;
    public Color stunColor = Color.purple;
    public Color enragedColor = new Color(1f, 0f, 0.5f);

    public Vector3 jumpscareLightOffset = new Vector3(0f, -1f, -0.5f);
    private Vector3 originalLightPosition;
    private float originalLightIntensity;
    private float originalLightRange;

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
    private float stateSoundTimer;

    private int lastPatrolIndex = -1;
    private bool isJumpscaring = false;
    private bool isGameOver = false;

    private bool reachedLastSeen = false;
    private float pointWaitTimer = 0f;
    private bool isSequenceRunning = false;

    private int animStateHash = Animator.StringToHash("State");

    public enum EnemyState { Roaming, Chasing, Searching, Jumpscare, Stunned, Staring, Enraged }
    public EnemyState currentState;
    private EnemyState previousState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCamScript = player.GetComponentInChildren<PlayerCam>();
        playerController = player.GetComponent<CharacterController>();

        if (enemyLight != null) enemyLight.enabled = true;

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
            originalLightIntensity = enemyLight.intensity;
            originalLightRange = enemyLight.range;
        }

        if (blackoutPanel != null) blackoutPanel.SetActive(false);

        patrolTimer = patrolWaitTime;
        currentState = EnemyState.Roaming;
        previousState = currentState;
        stateSoundTimer = Random.Range(minSoundInterval, maxSoundInterval);

        PickRoamPoint();
    }

    void Update()
    {
        if (isGameOver) return;

        anim.SetFloat("Speed", agent.velocity.magnitude);

        bool playerVisible = CanSeePlayer();

        if (!isJumpscaring && !isSequenceRunning && currentState != EnemyState.Stunned && Vector3.Distance(transform.position, player.position) <= jumpscareDistance)
        {
            StartJumpscare();
            return;
        }

        if (currentState != previousState && currentState != EnemyState.Jumpscare)
        {
            previousState = currentState;
            PlayStateSound();
            stateSoundTimer = Random.Range(minSoundInterval, maxSoundInterval);
        }
        else if (IsMobileState(currentState))
        {
            stateSoundTimer -= Time.deltaTime;
            if (stateSoundTimer <= 0f)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    PlayStateSound();
                }
                stateSoundTimer = Random.Range(minSoundInterval, maxSoundInterval);
            }
        }

        switch (currentState)
        {
            case EnemyState.Roaming:
                if (playerVisible) { StartChasing(); return; }

                if (enemyLight != null)
                {
                    enemyLight.color = roamColor;
                    enemyLight.intensity = originalLightIntensity;
                    enemyLight.range = originalLightRange;
                }
                anim.SetInteger(animStateHash, 0);
                Roam();
                break;

            case EnemyState.Enraged:
                if (playerVisible) { StartChasing(); return; }

                if (enemyLight != null)
                {
                    enemyLight.color = enragedColor;
                    enemyLight.intensity = originalLightIntensity * enragedIntensityMult;
                    enemyLight.range = originalLightRange * enragedRangeMult;
                }
                anim.SetInteger(animStateHash, 7);
                EnragedRoam();
                break;

            case EnemyState.Chasing:
                if (enemyLight != null)
                {
                    enemyLight.color = isEnraged ? enragedColor : chaseColor;
                    enemyLight.intensity = isEnraged ? (originalLightIntensity * enragedIntensityMult) : originalLightIntensity;
                    enemyLight.range = isEnraged ? (originalLightRange * enragedRangeMult) : originalLightRange;
                }
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
                        if (isEnraged)
                        {
                            currentState = EnemyState.Enraged;
                            PickRoamPoint();
                        }
                        else
                        {
                            currentState = EnemyState.Searching;
                            searchTimer = searchDuration;
                            reachedLastSeen = false;
                        }
                        return;
                    }
                }
                break;

            case EnemyState.Searching:
                if (playerVisible) { StartChasing(); return; }

                if (enemyLight != null) enemyLight.color = isEnraged ? enragedColor : searchColor;
                anim.SetInteger(animStateHash, 2);
                Search();
                break;

            case EnemyState.Stunned:
                if (enemyLight != null) enemyLight.color = stunColor;
                agent.isStopped = true;
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
                    agent.isStopped = false;
                    if (afterStunSound != null) audioSource.PlayOneShot(afterStunSound);

                    // Fix: Skip searching if waking up from a stun while enraged
                    if (isEnraged)
                    {
                        currentState = EnemyState.Enraged;
                        PickRoamPoint();
                    }
                    else
                    {
                        currentState = EnemyState.Searching;
                        searchTimer = searchDuration;
                        reachedLastSeen = false;
                    }
                }
                break;

            case EnemyState.Jumpscare:
                if (enemyLight != null)
                {
                    enemyLight.color = Color.red;
                    enemyLight.intensity = Random.Range(10f, 100f);
                }
                jumpscareTimer -= Time.deltaTime;
                if (jumpscareTimer <= 0f)
                {
                    if (enableCaptureFeature && !hasBeenCaptured)
                    {
                        hasBeenCaptured = true;
                        currentState = EnemyState.Staring;
                        StartCoroutine(CaptureSequence());
                    }
                    else
                    {
                        EndJumpscare();
                    }
                }
                break;

            case EnemyState.Staring:
                if (enemyLight != null) enemyLight.color = isEnraged ? enragedColor : searchColor;

                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                anim.SetInteger(animStateHash, 6);
                Vector3 lookDir = (player.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
                }
                break;
        }
    }

    private bool IsMobileState(EnemyState state)
    {
        return state == EnemyState.Roaming || state == EnemyState.Chasing || state == EnemyState.Searching || state == EnemyState.Enraged;
    }

    public void TriggerEnragedState()
    {
        if (currentState != EnemyState.Jumpscare && !isGameOver)
        {
            isEnraged = true;
            currentState = EnemyState.Enraged;
            PlayEnragedRoar();
        }
    }

    private void PlayEnragedRoar()
    {
        if (enragedRoarSound != null && audioSource != null)
        {
            float originalSpatialBlend = audioSource.spatialBlend;
            audioSource.spatialBlend = 0f;
            audioSource.PlayOneShot(enragedRoarSound);
            StartCoroutine(ResetSpatialBlend(originalSpatialBlend, enragedRoarSound.length));
        }
    }

    private IEnumerator ResetSpatialBlend(float originalBlend, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.spatialBlend = originalBlend;
    }

    public void StunEnemy()
    {
        if (isSequenceRunning || currentState == EnemyState.Jumpscare || isGameOver) return;

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

    void EnragedRoam()
    {
        agent.isStopped = false;
        agent.speed = enragedSpeed;
        agent.acceleration = 60f;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.3f)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0f)
            {
                PickRoamPoint();
                patrolTimer = enragedWaitTime;
            }
            else LookAround();
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.speed = isEnraged ? enragedSpeed : chaseSpeed;

        if (Vector3.Distance(agent.destination, player.position) > 1f)
            agent.SetDestination(player.position);
    }

    void Search()
    {
        agent.isStopped = false;
        agent.speed = isEnraged ? (enragedSpeed * 0.75f) : roamSpeed;
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0f)
        {
            currentState = isEnraged ? EnemyState.Enraged : EnemyState.Roaming;
            PickRoamPoint();
            return;
        }

        if (!reachedLastSeen)
        {
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
                    Vector3 randomDirection = Random.insideUnitSphere * searchRadius + lastSeenPosition;
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

            float score = (isEnraged) ? -distanceToPlayer : (Random.value * 20f - distanceToPlayer);
            if (score > bestScore) { bestScore = score; chosenIndex = i; }
        }
        if (chosenIndex != -1) { lastPatrolIndex = chosenIndex; agent.SetDestination(patrolPoints[chosenIndex].position); }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, player.position) > detectionRange) return false;
        if (Vector3.Angle(transform.forward, directionToPlayer) > fieldOfView / 2f) return false;
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, detectionRange))
            return hit.transform.root == player;
        return false;
    }

    void LookAround()
    {
        anim.SetInteger(animStateHash, 4);
        transform.Rotate(0, 40f * Time.deltaTime, 0);
    }

    void PlayStateSound()
    {
        if (isSequenceRunning || currentState == EnemyState.Staring || currentState == EnemyState.Jumpscare || audioSource == null)
            return;

        if (audioSource.isPlaying)
            return;

        AudioClip[] clipsToPlay = (currentState == EnemyState.Roaming) ? roamSounds :
                                  (currentState == EnemyState.Chasing || currentState == EnemyState.Enraged) ? chaseSounds :
                                  searchSounds;

        if (clipsToPlay != null && clipsToPlay.Length > 0)
        {
            audioSource.PlayOneShot(clipsToPlay[Random.Range(0, clipsToPlay.Length)]);
        }
    }

    void StartJumpscare()
    {
        if (isSequenceRunning || isGameOver) return;

        isSequenceRunning = true;
        isJumpscaring = true;
        currentState = EnemyState.Jumpscare;

        if (audioSource.isPlaying) audioSource.Stop();

        agent.isStopped = true;
        anim.SetInteger(animStateHash, 3);
        anim.SetTrigger("Jumpscare");

        jumpscareTimer = jumpscareDuration;

        if (enemyLight != null)
        {
            enemyLight.enabled = true;
            enemyLight.transform.localPosition = originalLightPosition + jumpscareLightOffset;
        }

        transform.rotation = Quaternion.LookRotation(new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z));

        if (jumpscareSound != null) audioSource.PlayOneShot(jumpscareSound);
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

    IEnumerator CaptureSequence()
    {
        if (forestAmbience != null) forestAmbience.SetActive(false);

        if (blackoutPanel != null)
        {
            blackoutPanel.SetActive(true);
            Image img = blackoutPanel.GetComponent<Image>();
            if (img != null) img.color = Color.black;
        }

        if (audioSource != null) audioSource.Stop();
        if (captureVoiceline != null) audioSource.PlayOneShot(captureVoiceline);
        if (shuttingCell != null) audioSource.PlayOneShot(shuttingCell);

        yield return new WaitForSeconds(1.0f);

        if (playerController != null) playerController.enabled = false;
        if (enemyPostCaptureSpawn != null) agent.Warp(enemyPostCaptureSpawn.position);

        if (playerPrisonSpawn != null)
        {
            player.position = playerPrisonSpawn.position;
            player.rotation = playerPrisonSpawn.rotation;
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        if (playerController != null) playerController.enabled = true;

        jumpscareCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        isJumpscaring = false;

        if (playerCamScript != null) playerCamScript.enabled = true;

        if (enemyLight != null)
        {
            enemyLight.transform.localPosition = originalLightPosition;
            enemyLight.intensity = originalLightIntensity;
            enemyLight.color = isEnraged ? enragedColor : searchColor;
        }

        if (blackoutPanel != null)
        {
            Image img = blackoutPanel.GetComponent<Image>();
            if (img != null)
            {
                float a = 1f;
                while (a > 0f)
                {
                    a -= Time.deltaTime * fadeInSpeed;
                    img.color = new Color(0, 0, 0, a);
                    yield return null;
                }
            }
            blackoutPanel.SetActive(false);
        }

        if (staringVoiceline != null)
        {
            audioSource.clip = staringVoiceline;
            audioSource.Play();
        }

        yield return new WaitForSeconds(stareDuration);

        if (enemyWalkAwayPoint != null) agent.SetDestination(enemyWalkAwayPoint.position);
        if (mainCanvas != null) mainCanvas.SetActive(true);

        currentState = isEnraged ? EnemyState.Enraged : EnemyState.Roaming;
        isSequenceRunning = false;

        if (forestAmbience != null) forestAmbience.SetActive(true);
    }

    void EndJumpscare()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (mainCanvas != null) mainCanvas.SetActive(true);
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (MapIcon != null) MapIcon.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StartChasing()
    {
        currentState = EnemyState.Chasing;
        lastSeenPosition = player.position;
        loseSightTimer = loseSightDelay;
    }
}
