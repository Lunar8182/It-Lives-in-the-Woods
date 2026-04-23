using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class WarningIntroKey : MonoBehaviour
{
    public CanvasGroup textGroup;
    public TMP_Text pressKeyText;

    public string nextScene = "IntroCutscene";
    public float fadeDuration = 2f;

    private bool canContinue = false;

    void Start()
    {
        if (textGroup != null)
            textGroup.alpha = 0f;

        pressKeyText.gameObject.SetActive(false);

        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(1f);

        pressKeyText.gameObject.SetActive(true);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            textGroup.alpha = t;
            yield return null;
        }

        textGroup.alpha = 1f;
        canContinue = true;
    }

    void Update()
    {
        if (!canContinue) return;

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}