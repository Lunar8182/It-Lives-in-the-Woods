using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpeningCutsceneManager : MonoBehaviour
{
    [Header("UI & World Objects")]
    public CanvasGroup fadeGroup;
    public GameObject roadSign;   // <--- NEW: Drag your green sign here!
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
        // Make sure the sign and enemy are hidden the moment the scene starts
        if (roadSign != null) roadSign.SetActive(false);
        if (enemy != null) enemy.SetActive(false);

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // --- PHASE 1: Fade In from Black ---
        float timer = 0;
        while (timer < 2f) // This now takes exactly 2 seconds to fade in
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = 1 - (timer / 2f);
            yield return null;
        }
        fadeGroup.alpha = 0;

        // --- PHASE 2: Driving to the Sign ---
        // We subtract 2 seconds from the wait time because the fade already took 2 seconds!
        yield return new WaitForSeconds(timeUntilSign - 2f);

        // Turn the sign on!
        if (roadSign != null) roadSign.SetActive(true);

        // --- PHASE 3: The Enemy Appears ---
        // Wait a few seconds for the player to read the sign...
        yield return new WaitForSeconds(delayBeforeEnemy);

        Vector3 carPos = Camera.main.transform.position;
        enemy.transform.position = new Vector3(carPos.x - 90f, carPos.y - 1.5f, carPos.z);
        enemy.SetActive(true);

        // --- PHASE 4: The Jumpscare Sequence ---
        yield return new WaitForSeconds(0.5f);
        clipSource.PlayOneShot(shockNoise);

        yield return new WaitForSeconds(1f);
        clipSource.PlayOneShot(voiceline);

        yield return new WaitForSeconds(0.5f);

        // Cut to black!
        fadeGroup.alpha = 1;
        crashSound.Play();
        drivingSound.Stop();

        yield return new WaitForSeconds(3f);

        clipSource.PlayOneShot(enemyLaugh);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainGame");
    }
}