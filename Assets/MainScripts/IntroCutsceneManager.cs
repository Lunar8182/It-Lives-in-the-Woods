using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpeningCutsceneManager : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public GameObject enemy;
    public AudioSource drivingSound;
    public AudioSource crashSound;
    public AudioSource clipSource;
    public AudioClip shockNoise;
    public AudioClip voiceline;
    public AudioClip enemyLaugh;
    public float driveDuration = 10f;

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        float timer = 0;
        while (timer < 10f)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = 1 - (timer / 2f);
            yield return null;
        }
        fadeGroup.alpha = 0;

        yield return new WaitForSeconds(driveDuration);


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