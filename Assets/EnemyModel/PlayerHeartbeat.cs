using UnityEngine;

public class PlayerHeartbeat : MonoBehaviour
{
    [Header("References")]
    public EnemyAI enemyAI; 
    public AudioSource heartbeatSource; 

    void Update()
    {
        if (enemyAI == null || heartbeatSource == null) return;

        if (enemyAI.currentState == EnemyAI.EnemyState.Chasing)
        {
            if (!heartbeatSource.isPlaying)
            {
                heartbeatSource.Play();
            }
        }
        else 
        {
            if (heartbeatSource.isPlaying)
            {
                heartbeatSource.Stop();
            }
        }
    }
}