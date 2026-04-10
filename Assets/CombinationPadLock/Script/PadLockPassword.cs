// Script by Marcelli Michele - Modified to prevent spamming
using System.Linq;
using UnityEngine;

public class PadLockPassword : MonoBehaviour
{
    MoveRuller _moveRull;
    public int[] _numberPassword = { 0, 0, 0, 0 };

    [HideInInspector]
    public bool isUnlocked = false; // Added this to track success

    private void Awake()
    {
        _moveRull = FindObjectOfType<MoveRuller>();
    }

    public void Password()
    {
        if (!isUnlocked && _moveRull._numberArray.SequenceEqual(_numberPassword))
        {
            isUnlocked = true; // Stop checking once solved

            // Here enter the event for the correct combination
            Debug.Log("Password correct!");

            // Below the for loop disables Blinking Material after the correct password
            for (int i = 0; i < _moveRull._rullers.Count; i++)
            {
                _moveRull._rullers[i].GetComponent<PadLockEmissionColor>()._isSelect = false;
                _moveRull._rullers[i].GetComponent<PadLockEmissionColor>().BlinkingMaterial();
            }

            // Optional: Auto-zoom out when the lock is solved
            if (_moveRull.mainCamera != null)
            {
                // You can add logic here to restore the camera FOV or trigger a door opening animation
            }
        }
    }
}