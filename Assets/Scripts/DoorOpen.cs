using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public enum DoorType { Normal, Prison }
    public DoorType doorType = DoorType.Normal;

    [Header("Movement Settings")]
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool isLocked = true;
    private bool isOpen = false;

    [Header("Visuals & UI")]
    public GameObject Lock;
    public GameObject lockedMessage;

    [Header("Audio Settings")]
    public AudioClip normalDoorSound; 
    public AudioClip prisonDoorSound; 
    private AudioSource audioSource;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1.0f; 
        audioSource.playOnAwake = false;
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
            if (doorType == DoorType.Normal && InventoryManager.instance.hasKey)
            {
                Unlock();
            }
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

        if (doorType == DoorType.Normal && normalDoorSound != null)
        {
            audioSource.PlayOneShot(normalDoorSound);
        }
        else if (doorType == DoorType.Prison && prisonDoorSound != null)
        {
            audioSource.PlayOneShot(prisonDoorSound);
        }

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
            StopAllCoroutines();
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