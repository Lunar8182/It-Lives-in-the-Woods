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

    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 roamPoint;
    private bool chasing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        PickRoamPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer()) {
            chasing = true;
        }

        if (chasing) {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }

        else {
            agent.speed = roamSpeed;

            if (!agent.pathPending && agent.remainingDistance < 0.5f) {
                PickRoamPoint();
            }
        }
    }

    void PickRoamPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
        {
            roamPoint = hit.position;
            agent.SetDestination(roamPoint);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle < fieldOfView / 2f)
            {
                Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, detectionRange))
                {
                    if (hit.transform == player)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
