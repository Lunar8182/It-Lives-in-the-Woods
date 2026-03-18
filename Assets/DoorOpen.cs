using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpen)
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
    }

    public void Interact()
    {
        ToggleDoor();
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 doorToPlayer = player.position - transform.position;
            float direction = Vector3.Dot(transform.right, doorToPlayer);

            if (direction > 0)
                openRotation = Quaternion.Euler(0, transform.eulerAngles.y + openAngle, 0);
            else
                openRotation = Quaternion.Euler(0, transform.eulerAngles.y - openAngle, 0);
        }
    }
}