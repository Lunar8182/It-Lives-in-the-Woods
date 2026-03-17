using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float roamRadius = 100f;
    public float detectionRange = 50f;
    public float fieldOfView = 90f;
    public float chaseSpeed = 12f;
    public float roamSpeed = 3f;
    public float searchDuration = 5f;
    public float loseSightDelay = 2f;

    public Animator anim;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private Vector3 lastSeenPosition;
    private Vector3 roamPoint;

    private float searchTimer;
    private float loseSightTimer;

    public float patrolWaitTime = 3f;
    private float patrolTimer;

    int lastPatrolIndex = -1;

    enum EnemyState
    {
        Roaming, Chasing, Searching
    }

    private EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolTimer = patrolWaitTime;
        currentState = EnemyState.Roaming;
        PickRoamPoint();
    }

    void Update()
    {
        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * detectionRange, Color.red);

        anim.SetFloat("Speed", agent.velocity.magnitude);

        bool playerVisible = CanSeePlayer();

        switch (currentState)
        {
            case EnemyState.Roaming:
                Roam();

                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }

                break;

            case EnemyState.Chasing:
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
                Search();

                if (playerVisible)
                {
                    lastSeenPosition = player.position;
                    currentState = EnemyState.Chasing;
                }

                break;
        }
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
            else
            {
                LookAround();
            }
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;

        if (Vector3.Distance(agent.destination, player.position) > 1f)
        {
            agent.SetDestination(player.position);
        }
    }

    void Search()
    {
        agent.speed = roamSpeed;

        if (agent.destination != lastSeenPosition)
        {
            agent.SetDestination(lastSeenPosition);
        }

        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0)
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
        Debug.DrawRay(transform.position + Vector3.up * 1f, directionToPlayer * detectionRange, Color.green);

        if (distance < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle < fieldOfView / 2f)
            {
                Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, detectionRange))
                {
                    if (hit.transform.root == player)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void LookAround()
    {
        transform.Rotate(0, 40f * Time.deltaTime, 0);
    }

    // debugger - shows ai movement
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
