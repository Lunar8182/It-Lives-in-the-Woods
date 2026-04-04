using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public DoorType doorType = DoorType.Normal;
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool isLocked = true;
    private bool isOpen = false;
    public GameObject Lock;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    public GameObject lockedMessage;
    public enum DoorType { Normal, Prison }

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
        if (isLocked)
        {
            // Logic for Normal Doors
            if (doorType == DoorType.Normal && InventoryManager.instance.hasKey)
            {
                Unlock();
            }
            // Logic for Prison Door
            else if (doorType == DoorType.Prison && InventoryManager.instance.hasPrisonKey)
            {
                Unlock();
            }
            else
            {
                ShowLockedMessage();
                return;
            }
        }
        else
        {
            ToggleDoor();
        }
    }

    void Unlock()
    {
        isLocked = false;
        if (Lock != null) Destroy(Lock);
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

    void ShowLockedMessage()
    {
        if (lockedMessage != null)
        {
            StartCoroutine(HideLockedMessage());
        }
    }

    IEnumerator HideLockedMessage()
    {
        lockedMessage.SetActive(true);
        yield return new WaitForSeconds(2f);
        lockedMessage.SetActive(false);
    }
}