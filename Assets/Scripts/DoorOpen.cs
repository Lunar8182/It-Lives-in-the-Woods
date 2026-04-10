using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public enum DoorType { Normal, Prison, ComboLock }
    public enum KeyColor { None, Red, Yellow } // --- NEW: Color Dropdown ---

    [Header("Door Settings")]
    public DoorType doorType = DoorType.Normal;
    [Tooltip("What color key does this door need? (Only applies if Door Type is Normal)")]
    public KeyColor requiredKeyColor = KeyColor.None; // --- NEW ---

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

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        // --- NEW: Automatically color the lock object to match the required key! ---
        if (Lock != null && requiredKeyColor != KeyColor.None)
        {
            Renderer lockRenderer = Lock.GetComponent<Renderer>();
            if (lockRenderer != null)
            {
                if (requiredKeyColor == KeyColor.Red)
                    lockRenderer.material.color = Color.red;
                else if (requiredKeyColor == KeyColor.Yellow)
                    lockRenderer.material.color = Color.yellow;
            }
        }
    }

    void Update()
    {
        // Smooth rotation
        if (isOpen)
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
    }

    public void Interact()
    {
        if (isLocked)
        {
            // --- NEW: Checking for colored keys ---
            if (doorType == DoorType.Normal)
            {
                if (requiredKeyColor == KeyColor.Red && InventoryManager.instance.hasRedKey)
                    Unlock();
                else if (requiredKeyColor == KeyColor.Yellow && InventoryManager.instance.hasYellowKey)
                    Unlock();
                else if (requiredKeyColor == KeyColor.None && InventoryManager.instance.hasKey)
                    Unlock(); // Legacy check for your basic keys
                else
                    ShowLockedMessage();
            }
            else if (doorType == DoorType.Prison && InventoryManager.instance.hasRedKey)
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

        if (Lock != null)
            Destroy(Lock);

        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (doorType == DoorType.Normal && normalDoorSound != null)
            audioSource.PlayOneShot(normalDoorSound);
        else if (doorType == DoorType.Prison && prisonDoorSound != null)
            audioSource.PlayOneShot(prisonDoorSound);

        if (isOpen)
        {
            if (player == null) return;

            Vector3 doorToPlayer = player.position - transform.position;
            float direction = Vector3.Dot(transform.right, doorToPlayer);

            if (direction > 0)
                openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
            else
                openRotation = closedRotation * Quaternion.Euler(0, -openAngle, 0);
        }
    }

    public void ForceOpenFromPuzzle()
    {
        isLocked = false;

        if (Lock != null) Destroy(Lock);

        if (!isOpen)
        {
            isOpen = true;

            if (doorType == DoorType.Normal && normalDoorSound != null)
                audioSource.PlayOneShot(normalDoorSound);
            else if (doorType == DoorType.Prison && prisonDoorSound != null)
                audioSource.PlayOneShot(prisonDoorSound);

            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
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