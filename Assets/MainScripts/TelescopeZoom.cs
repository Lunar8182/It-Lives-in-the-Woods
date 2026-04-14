using System.Collections;
using UnityEngine;

public class TelescopeController : MonoBehaviour
{
    public Camera playerCamera;

    public float normalFOV = 60f;
    public float zoomFOV = 15f;
    public float zoomSpeed = 2f;
    public GameObject[] telescopeTargets;

    private bool usingTelescope = false;
    private Coroutine currentZoomCoroutine;

    // NEW: Tracks exactly when the telescope was opened
    private float timeOpened;

    void Update()
    {
        // NEW: Only allow exiting if at least 0.2 seconds have passed since opening
        if (usingTelescope && Input.GetKeyDown(KeyCode.E) && Time.time > timeOpened + 0.2f)
        {
            ExitTelescope();
        }
    }

    public void UseTelescope()
    {
        usingTelescope = true;

        // Record the exact time we opened it
        timeOpened = Time.time;

        if (currentZoomCoroutine != null) StopCoroutine(currentZoomCoroutine);
        currentZoomCoroutine = StartCoroutine(ZoomCamera(zoomFOV));

        foreach (GameObject target in telescopeTargets)
        {
            target.SetActive(true);
        }
    }

    public void ExitTelescope()
    {
        usingTelescope = false;

        if (currentZoomCoroutine != null) StopCoroutine(currentZoomCoroutine);
        currentZoomCoroutine = StartCoroutine(ZoomCamera(normalFOV));

        foreach (GameObject target in telescopeTargets)
        {
            target.SetActive(false);
        }
    }

    IEnumerator ZoomCamera(float targetFOV)
    {
        float startFOV = playerCamera.fieldOfView;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * zoomSpeed;
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }
    }
}