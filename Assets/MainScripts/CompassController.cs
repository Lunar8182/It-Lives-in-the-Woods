using UnityEngine;

public class CompassController : MonoBehaviour
{
    [Header("3D World Reference")]
    public Transform playerTransform;

    [Header("2D UI Reference")]
    public RectTransform playerArrowUI;

    [Header("Settings")]
    public float orientationOffset = 90f;

    void Update()
    {
        if (playerTransform == null || playerArrowUI == null) return;

        float playerHeading = playerTransform.eulerAngles.y;

        playerArrowUI.localEulerAngles = new Vector3(0, 0, -playerHeading + orientationOffset);
    }
}