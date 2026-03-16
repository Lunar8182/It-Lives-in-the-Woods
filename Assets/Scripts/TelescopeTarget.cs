using UnityEngine;

public class TelescopeTarget : MonoBehaviour
{
    public AudioSource voiceLine;

    private bool triggered = false;

    void Update()
    {
        Camera cam = Camera.main;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.gameObject == gameObject && !triggered)
            {
                triggered = true;
                voiceLine.Play();
            }
        }
    }
}