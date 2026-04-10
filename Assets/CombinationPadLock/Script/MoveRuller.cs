// Script by Marcelli Michele - Modified for Zoom, Arrow Keys, and Letter-style Player Freezing
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

    [HideInInspector]
    public List<GameObject> _rullers = new List<GameObject>();
    private int _scroolRuller = 0;
    private int _changeRuller = 0;

    [HideInInspector]
    public int[] _numberArray = { 0, 0, 0, 0 };

    private int _numberRuller = 0;
    private bool _isActveEmission = false;

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
    }

    void Update()
    {
        if (_lockPassword.isUnlocked) return;

        HandleZoom();
        UpdateEmissionVisuals();

        if (isZoomedIn)
        {
            MoveRulles();
            RotateRullers();
            _lockPassword.Password();
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
            // EXACT LOGIC FROM YOUR LETTER SCRIPT: FREEZE
            if (playerController != null) playerController.enabled = false;

            if (cameraObject != null)
            {
                MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts)
                {
                    s.enabled = false;
                }
            }
        }
        else
        {
            // EXACT LOGIC FROM YOUR LETTER SCRIPT: UNFREEZE
            if (playerController != null) playerController.enabled = true;

            if (cameraObject != null)
            {
                MonoBehaviour[] scripts = cameraObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts)
                {
                    s.enabled = true;
                }
            }

            // Turn off emission completely if we zoom out
            _isActveEmission = false;
            foreach (var r in _rullers)
            {
                var emissionScript = r.GetComponent<PadLockEmissionColor>();
                if (emissionScript != null)
                {
                    emissionScript._isSelect = false;
                    emissionScript.BlinkingMaterial();
                }
            }
        }
    }

    void MoveRulles()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _isActveEmission = true;
            _changeRuller++;
            _numberRuller += 1;
            if (_numberRuller > 3) _numberRuller = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _isActveEmission = true;
            _changeRuller--;
            _numberRuller -= 1;
            if (_numberRuller < 0) _numberRuller = 3;
        }

        _changeRuller = (_changeRuller + _rullers.Count) % _rullers.Count;
    }

    void UpdateEmissionVisuals()
    {
        for (int i = 0; i < _rullers.Count; i++)
        {
            if (_isActveEmission)
            {
                var emissionScript = _rullers[i].GetComponent<PadLockEmissionColor>();
                emissionScript._isSelect = (_changeRuller == i);
                emissionScript.BlinkingMaterial();
            }
        }
    }

    void RotateRullers()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _isActveEmission = true;
            _scroolRuller = 36;
            _rullers[_changeRuller].transform.Rotate(-_scroolRuller, 0, 0, Space.Self);

            _numberArray[_changeRuller] += 1;
            if (_numberArray[_changeRuller] > 9) _numberArray[_changeRuller] = 0;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _isActveEmission = true;
            _scroolRuller = 36;
            _rullers[_changeRuller].transform.Rotate(_scroolRuller, 0, 0, Space.Self);

            _numberArray[_changeRuller] -= 1;
            if (_numberArray[_changeRuller] < 0) _numberArray[_changeRuller] = 9;
        }
    }
}