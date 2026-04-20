using UnityEngine;

public class CompassController : MonoBehaviour
{
    [Header("3D World Reference")]
    public Transform playerTransform;

    [Header("2D UI Reference")]
    public RectTransform playerArrowUI;

    [Header("Settings")]
    public float orientationOffset = 90f; // Add 90 degrees if your image points right by default

    void Update()
    {
        // Safety check to prevent errors if something isn't assigned
        if (playerTransform == null || playerArrowUI == null) return;

        float playerHeading = playerTransform.eulerAngles.y;

        // Apply the rotation with the offset
        playerArrowUI.localEulerAngles = new Vector3(0, 0, -playerHeading + orientationOffset);
    }
}