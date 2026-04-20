using UnityEngine;

public class ApplyPhysicsMaterial : MonoBehaviour
{
    public PhysicsMaterial frictionlessMaterial;

    void Awake()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider col in allColliders)
        {
            if (!col.isTrigger)
            {
                col.sharedMaterial = frictionlessMaterial;
            }
        }
    }
}