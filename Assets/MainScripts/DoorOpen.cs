using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public enum DoorType { Normal, Prison, ComboLock, Lockpick }
    public enum KeyColor { None, Red, Yellow }

    [Header("Door Settings")]
    public DoorType doorType = DoorType.Normal;
    public KeyColor requiredKeyColor = KeyColor.None;

    [Header("References")]
    public GameObject GameHUD;
    public GameObject EButton;

    [Header("Lockpick Settings")]
    public LockpickQTE lockpickMinigame;
    public EnemyAI enemyScript;

    [Header("Movement Settings")]
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool isLocked = true;

    private bool isOpen = false;
    private bool isPickingLock = false;

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
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;

        if (Lock != null && requiredKeyColor != KeyColor.None)
        {
            Renderer lockRenderer = Lock.GetComponent<Renderer>();
            if (lockRenderer != null)
            {
                if (requiredKeyColor == KeyColor.Red) lockRenderer.material.color = Color.red;
                else if (requiredKeyColor == KeyColor.Yellow) lockRenderer.material.color = Color.yellow;
            }
        }

        if (lockpickMinigame != null)
        {
            lockpickMinigame.onGameWin.AddListener(Unlock);

            lockpickMinigame.onGameExit.AddListener(UnfreezePlayer);
        }
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
            if (doorType == DoorType.Lockpick)
            {
                if (enemyScript != null && !enemyScript.hasBeenCaptured)
                {
                    ShowLockedMessage();
                    return;
                }

                if (lockpickMinigame != null)
                {
                    isPickingLock = true;

                    lockpickMinigame.StartGame();
                    if (GameHUD != null) GameHUD.SetActive(false);
                    if (EButton != null) EButton.SetActive(false);

                    if (player != null)
                    {
                        MonoBehaviour pm = player.GetComponent("PlayerMovement") as MonoBehaviour;
                        if (pm != null) pm.enabled = false;

                        MonoBehaviour[] scripts = player.GetComponentsInChildren<MonoBehaviour>();
                        foreach (MonoBehaviour s in scripts)
                        {
                            string scriptName = s.GetType().Name;
                            if (scriptName.Contains("MouseLook") || scriptName.Contains("CameraLook") || scriptName.Contains("PlayerCam"))
                            {
                                s.enabled = false;
                            }
                        }
                    }

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else if (doorType == DoorType.Normal)
            {
                if (requiredKeyColor == KeyColor.Red && InventoryManager.instance.hasRedKey) Unlock();
                else if (requiredKeyColor == KeyColor.Yellow && InventoryManager.instance.hasYellowKey) Unlock();
                else if (requiredKeyColor == KeyColor.None && InventoryManager.instance.hasKey) Unlock();
                else ShowLockedMessage();
            }
            else if (doorType == DoorType.Prison && InventoryManager.instance.hasRedKey)
            {
                Unlock();
            }
            else
            {
                ShowLockedMessage();
            }
        }
        else
        {
            ToggleDoor();
        }
    }

    public void Unlock()
    {
        isLocked = false;
        if (Lock != null) Destroy(Lock);

        UnfreezePlayer();
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (doorType == DoorType.Normal && normalDoorSound != null)
            audioSource.PlayOneShot(normalDoorSound);
        else if ((doorType == DoorType.Prison || doorType == DoorType.Lockpick) && prisonDoorSound != null)
            audioSource.PlayOneShot(prisonDoorSound);

        if (isOpen && player != null)
        {
            Vector3 doorToPlayer = player.position - transform.position;
            float direction = Vector3.Dot(transform.right, doorToPlayer);
            openRotation = (direction > 0) ? closedRotation * Quaternion.Euler(0, openAngle, 0) : closedRotation * Quaternion.Euler(0, -openAngle, 0);
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

    public void UnfreezePlayer()
    {
        isPickingLock = false;

        if (GameHUD != null) GameHUD.SetActive(true);
        if (EButton != null) EButton.SetActive(true);

        if (player != null)
        {
            MonoBehaviour pm = player.GetComponent("PlayerMovement") as MonoBehaviour;
            if (pm != null) pm.enabled = true;

            MonoBehaviour[] scripts = player.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts)
            {
                string scriptName = s.GetType().Name;
                if (scriptName.Contains("MouseLook") || scriptName.Contains("CameraLook") || scriptName.Contains("PlayerCam"))
                {
                    s.enabled = true;
                }
            }
        }
    }
    void LateUpdate()
    {
        if (isPickingLock)
        {
            if (EButton != null) EButton.SetActive(false);
            if (GameHUD != null) GameHUD.SetActive(false);
        }
    }
}