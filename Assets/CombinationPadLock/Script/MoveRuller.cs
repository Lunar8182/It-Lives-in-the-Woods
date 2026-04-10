// Script by Marcelli Michele - Modified to snap Camera back on Destroy
using System.Collections.Generic;
using UnityEngine;

public class MoveRuller : MonoBehaviour
{
    PadLockPassword _lockPassword;

    [Header("Camera Zoom Settings")]
    public Camera mainCamera;
    public float zoomFOV = 30f;
    public float zoomSpeed = 5f;
    private float normalFOV;
    private bool isZoomedIn = false;

    [Header("Player & Camera Objects (To Freeze)")]
    public MonoBehaviour playerController;
    public GameObject cameraObject;

    [Header("Selection Pointer Settings")]
    public Transform selectionPointer;
    public Vector3 pointerOffset = new Vector3(0, 0.05f, 0);

    [HideInInspector]
    public List<GameObject> _rullers = new List<GameObject>();
    private int _scroolRuller = 0;
    private int _changeRuller = 0;

    [HideInInspector]
    public int[] _numberArray = { 0, 0, 0, 0 };
    private int _numberRuller = 0;

    void Awake()
    {
        _lockPassword = FindObjectOfType<PadLockPassword>();

        _rullers.Add(GameObject.Find("Ruller1"));
        _rullers.Add(GameObject.Find("Ruller2"));
        _rullers.Add(GameObject.Find("Ruller3"));
        _rullers.Add(GameObject.Find("Ruller4"));

        foreach (GameObject r in _rullers)
        {
            r.transform.Rotate(-144, 0, 0, Space.Self);
        }

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) normalFOV = mainCamera.fieldOfView;

        if (selectionPointer != null) selectionPointer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_lockPassword.isUnlocked) return;

        HandleZoom();

        if (isZoomedIn)
        {
            MoveRulles();
            RotateRullers();
            _lockPassword.Password();
            UpdatePointerPosition();
        }
    }

    void HandleZoom()
    {
        if (mainCamera == null) return;

        float targetFOV = isZoomedIn ? zoomFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

        if (isZoomedIn && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            ToggleZoom();
        }
    }

    public void ToggleZoom()
    {
        isZoomedIn = !isZoomedIn;

        if (isZoomedIn)
        {
            if (playerController != null) playerController.enabled = false;

            if (cameraObject != null)
            {
                MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts) s.enabled = false;
            }

            if (selectionPointer != null)
            {
                selectionPointer.gameObject.SetActive(true);
                UpdatePointerPosition();
            }
        }
        else
        {
            if (playerController != null) playerController.enabled = true;

            if (cameraObject != null)
            {
                MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts) s.enabled = true;
            }

            if (selectionPointer != null) selectionPointer.gameObject.SetActive(false);
        }
    }

    void MoveRulles()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _changeRuller++;
            _numberRuller += 1;
            if (_numberRuller > 3) _numberRuller = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _changeRuller--;
            _numberRuller -= 1;
            if (_numberRuller < 0) _numberRuller = 3;
        }

        _changeRuller = (_changeRuller + _rullers.Count) % _rullers.Count;
    }

    void UpdatePointerPosition()
    {
        if (selectionPointer != null && _rullers.Count > 0)
        {
            selectionPointer.position = _rullers[_changeRuller].transform.position;
            selectionPointer.position += transform.TransformDirection(pointerOffset);
        }
    }

    void RotateRullers()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _scroolRuller = 36;
            _rullers[_changeRuller].transform.Rotate(-_scroolRuller, 0, 0, Space.Self);

            _numberArray[_changeRuller] += 1;
            if (_numberArray[_changeRuller] > 9) _numberArray[_changeRuller] = 0;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _scroolRuller = 36;
            _rullers[_changeRuller].transform.Rotate(_scroolRuller, 0, 0, Space.Self);

            _numberArray[_changeRuller] -= 1;
            if (_numberArray[_changeRuller] < 0) _numberArray[_changeRuller] = 9;
        }
    }

    // --- NEW SAFETY NET ---
    // This runs exactly when Destroy(gameObject) triggers in the other script
    private void OnDestroy()
    {
        // 1. Force the camera FOV back to normal instantly
        if (mainCamera != null && normalFOV > 0)
        {
            mainCamera.fieldOfView = normalFOV;
        }

        // 2. Double-check that player movement is turned back on
        if (playerController != null) playerController.enabled = true;

        if (cameraObject != null)
        {
            MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts) s.enabled = true;
        }
    }
}