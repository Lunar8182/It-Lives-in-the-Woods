using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 3.0f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; 
    }

    void Start()
    {
        StartCoroutine(FadeInFromBlack());
    }

    IEnumerator FadeInFromBlack()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false); 
    }
}