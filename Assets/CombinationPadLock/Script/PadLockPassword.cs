// Script by Marcelli Michele - Modified to trigger DoorInteract
using System.Linq;
using UnityEngine;

public class PadLockPassword : MonoBehaviour
{
    MoveRuller _moveRull;
    public int[] _numberPassword = { 0, 0, 0, 0 };

    [HideInInspector]
    public bool isUnlocked = false;

    [Header("Unlock Events")]
    [Tooltip("Drag the door you want to open into this slot")]
    public DoorInteract connectedDoor; // --- NEW ---

    public AudioClip unlockSound;
    public float destroyDelay = 0.5f;

    private void Awake()
    {
        _moveRull = FindObjectOfType<MoveRuller>();
    }

    public void Password()
    {
        if (!isUnlocked && _moveRull._numberArray.SequenceEqual(_numberPassword))
        {
            isUnlocked = true;

            Debug.Log("Password correct! Opening door and destroying lock.");

            // 1. Play the sound
            if (unlockSound != null)
            {
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);
            }

            // 2. Zoom out
            if (_moveRull != null)
            {
                _moveRull.ToggleZoom();

                if (_moveRull.selectionPointer != null)
                {
                    _moveRull.selectionPointer.gameObject.SetActive(false);
                }
            }

            // 3. --- NEW: FORCE THE DOOR OPEN ---
            if (connectedDoor != null)
            {
                connectedDoor.ForceOpenFromPuzzle();
            }

            // 4. Destroy the lock after a tiny delay
            Destroy(gameObject, destroyDelay);
        }
    }
}