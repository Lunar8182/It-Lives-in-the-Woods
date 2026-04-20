using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour
{
    [Header("Map Settings")]
    public RectTransform mapPanel;
    public float slideDuration = 0.4f;
    public GameObject compass;

    [Header("Positions")]
    // (0,0) is the center of the screen if anchored correctly
    public Vector2 onScreenPosition = new Vector2(0, 0);
    // -1000 sends it down off the bottom of the screen. You may need to tweak this depending on your resolution.
    public Vector2 offScreenPosition = new Vector2(0, -1000);

    private bool isMapOpen = false;
    private Coroutine slideCoroutine;

    void Start()
    {
        // Ensure the map starts off-screen when the game begins
        if (mapPanel != null)
        {
            mapPanel.anchoredPosition = offScreenPosition;
        }
    }

    void Update()
    {
        // Check for the M key press
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        isMapOpen = !isMapOpen;

        if (compass != null)
        {
            compass.SetActive(isMapOpen);
        }

        // If a slide is already happening, stop it so we can reverse directions smoothly
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        // Determine where we are going
        Vector2 targetPosition = isMapOpen ? onScreenPosition : offScreenPosition;

        // Start the sliding animation
        slideCoroutine = StartCoroutine(SlideMap(targetPosition));
    }

    IEnumerator SlideMap(Vector2 targetPosition)
    {
        Vector2 startPosition = mapPanel.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            // Time.deltaTime ensures the animation plays at a consistent speed regardless of framerate
            elapsedTime += Time.deltaTime;

            // Calculate how far along the animation we are (0.0 to 1.0)
            float t = elapsedTime / slideDuration;

            // Optional: Adds a "Smooth Step" easing so it slows down nicely at the end of the slide
            t = t * t * (3f - 2f * t);

            // Move the panel
            mapPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            // Wait until the next frame before continuing the loop
            yield return null;
        }

        // Ensure it snaps exactly to the target at the very end
        mapPanel.anchoredPosition = targetPosition;
    }
}