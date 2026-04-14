using UnityEngine;

public class AutoDrive : MonoBehaviour
{
    [Header("Driving Settings")]
    public float speed = 20f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}