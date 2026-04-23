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
    public DoorInteract connectedDoor;

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

            if (unlockSound != null)
            {
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);
            }

            if (_moveRull != null)
            {
                if (_moveRull != null && _moveRull.IsZoomedIn())
                {
                    _moveRull.ForceExitZoom();
                }

                if (_moveRull.selectionPointer != null)
                {
                    _moveRull.selectionPointer.gameObject.SetActive(false);
                }
            }

            if (connectedDoor != null)
            {
                connectedDoor.ForceOpenFromPuzzle();
            }

            Destroy(gameObject, destroyDelay);
        }
    }
}