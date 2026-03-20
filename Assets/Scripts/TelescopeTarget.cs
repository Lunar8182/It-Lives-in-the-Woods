using UnityEngine;

public class TelescopeTarget : MonoBehaviour
{
    public AudioSource voiceLine;
    public float raycastDistance = 1000f; // Increased for telescope distances

    private bool triggered = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (triggered || cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                triggered = true;

                if (voiceLine != null && !voiceLine.isPlaying)
                {
                    voiceLine.Play();
                }
            }
        }
    }
}