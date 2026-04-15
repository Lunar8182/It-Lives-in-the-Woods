using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpeningCutsceneManager : MonoBehaviour
{
    [Header("UI & World Objects")]
    public CanvasGroup fadeGroup;
    public GameObject roadSign; 
    public GameObject enemy;

    [Header("Audio Sources & Clips")]
    public AudioSource drivingSound;
    public AudioSource crashSound;
    public AudioSource clipSource;
    public AudioClip shockNoise;
    public AudioClip voiceline;
    public AudioClip enemyLaugh;

    [Header("Cutscene Timing")]
    [Tooltip("How many seconds from the start of the scene until the sign appears")]
    public float timeUntilSign = 10f;
    [Tooltip("How many seconds pass after the sign appears before the enemy spawns")]
    public float delayBeforeEnemy = 5f;

    void Start()
    {
        if (roadSign != null) roadSign.SetActive(false);
        if (enemy != null) enemy.SetActive(false);

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        float timer = 0;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = 1 - (timer / 2f);
            yield return null;
        }
        fadeGroup.alpha = 0;

        yield return new WaitForSeconds(timeUntilSign - 2f);

        if (roadSign != null) roadSign.SetActive(true);

        yield return new WaitForSeconds(delayBeforeEnemy);

        Vector3 carPos = Camera.main.transform.position;
        enemy.transform.position = new Vector3(carPos.x - 90f, carPos.y - 1.5f, carPos.z);
        enemy.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        clipSource.PlayOneShot(shockNoise);

        yield return new WaitForSeconds(1f);
        clipSource.PlayOneShot(voiceline);

        yield return new WaitForSeconds(0.5f);

        fadeGroup.alpha = 1;
        crashSound.Play();
        drivingSound.Stop();

        yield return new WaitForSeconds(3f);

        clipSource.PlayOneShot(enemyLaugh);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainGame");
    }
}