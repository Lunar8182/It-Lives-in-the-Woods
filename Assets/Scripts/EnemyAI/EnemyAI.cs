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

    private NavMeshAgent agent;
    private Vector3 lastSeenPosition;
    private Vector3 roamPoint;

    private float searchTimer;
    private float loseSightTimer;

    enum EnemyState
    {
        Roaming, Chasing, Searching
    }

    private EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    PickRoamPoint();
                }
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
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0;
        randomDirection += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            roamPoint = hit.position;
            agent.SetDestination(roamPoint);
        }
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
