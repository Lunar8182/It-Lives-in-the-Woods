using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TelescopeController : MonoBehaviour
{
    public Camera playerCamera;

    public float normalFOV = 60f;
    public float zoomFOV = 15f;
    public GameObject[] telescopeTargets;
    private bool usingTelescope = false;

    void Update()
    {
        if (usingTelescope && Input.GetKeyDown(KeyCode.E))
        {
            ExitTelescope();
        }
    }

    public void UseTelescope()
    {
        usingTelescope = true;
        StartCoroutine(ZoomIn());

        foreach (GameObject target in telescopeTargets)
        {
            target.SetActive(true);
        }
    }

    public void ExitTelescope()
    {
        usingTelescope = false;
        playerCamera.fieldOfView = normalFOV;

        foreach (GameObject target in telescopeTargets)
        {
            target.SetActive(false);
        }
    }

    IEnumerator ZoomIn()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 2;
            playerCamera.fieldOfView = Mathf.Lerp(normalFOV, zoomFOV, t);
            yield return null;
        }
    }
}