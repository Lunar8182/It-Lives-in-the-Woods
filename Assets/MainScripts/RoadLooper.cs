using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    public Transform car;
    public float terrainWidth = 1000f; 
    public int totalSegments = 2; 
    public float loadEarlyOffset = 200f; 

    void Update()
    {
        if (car.position.x < (transform.position.x - terrainWidth) + loadEarlyOffset)
        {
            Vector3 newPos = transform.position;
            newPos.x -= (terrainWidth * totalSegments);
            transform.position = newPos;
        }
    }
}