using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour
{
    [Header("Map Settings")]
    public RectTransform mapPanel;
    public float slideDuration = 0.4f;
    public GameObject compass;

    [Header("Positions")]
    public Vector2 onScreenPosition = new Vector2(0, 0);
    public Vector2 offScreenPosition = new Vector2(0, -1000);

    private bool isMapOpen = false;
    private Coroutine slideCoroutine;

    void Start()
    {
        if (mapPanel != null)
        {
            mapPanel.anchoredPosition = offScreenPosition;
        }
    }

    void Update()
    {
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

        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        Vector2 targetPosition = isMapOpen ? onScreenPosition : offScreenPosition;

        slideCoroutine = StartCoroutine(SlideMap(targetPosition));
    }

    IEnumerator SlideMap(Vector2 targetPosition)
    {
        Vector2 startPosition = mapPanel.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / slideDuration;

            t = t * t * (3f - 2f * t);

            mapPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        mapPanel.anchoredPosition = targetPosition;
    }
}