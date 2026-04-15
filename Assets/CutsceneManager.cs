using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CarCutsceneManager : MonoBehaviour
{
    [Header("Sequence Timing")]
    public float introMusicDuration = 10f;
    public float radioSilenceDuration = 4f;
    public float stareTimeBeforeJumpscare = 3f;
    public float lookAngleThreshold = 45f;

    [Header("New Music Sequence")]
    public AudioClip introMusic;        
    public AudioClip radioSilenceSound; 
    public AudioClip sinisterMusic;     

    [Header("References")]
    public GameObject enemyDummy;
    public Animator enemyAnimator;
    public CanvasGroup fadeGroup;

    [Header("Cameras")]
    public Camera drivingCamera;
    public Camera jumpscareCamera;

    [Header("Audio Sources (Drag these in!)")]
    [Tooltip("The AudioSource physically attached to your Radio object")]
    public AudioSource radioAudioSource;
    [Tooltip("The AudioSource attached to this Cutscene Manager or the Enemy")]
    public AudioSource enemyAudioSource;

    [Header("Enemy Audio SFX")]
    public AudioClip voicelineSound;    
    public AudioClip jumpscareSound;

    [Header("End Scene")]
    public string nextSceneName = "GameOverScene";

    private bool enemySpawned = false;
    private bool jumpscareTriggered = false;

    void Start()
    {
        if (enemyDummy != null) enemyDummy.SetActive(false);
        if (jumpscareCamera != null) jumpscareCamera.gameObject.SetActive(false);

        if (enemyAudioSource == null) enemyAudioSource = gameObject.AddComponent<AudioSource>();

        StartCoroutine(CarSpawnSequence());
    }

    void Update()
    {
        if (enemySpawned && !jumpscareTriggered)
        {
            CheckIfPlayerIsLooking();
        }
    }

    private IEnumerator CarSpawnSequence()
    {
        float timer = 0;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = 1 - (timer / 2f);
            yield return null;
        }
        fadeGroup.alpha = 0;


        if (radioAudioSource != null && introMusic != null)
        {
            radioAudioSource.clip = introMusic;
            radioAudioSource.Play();
        }

        yield return new WaitForSeconds(introMusicDuration);

        if (radioAudioSource != null)
        {
            if (radioSilenceSound != null)
            {
                radioAudioSource.clip = radioSilenceSound;
                radioAudioSource.Play();
            }
            else
            {
                radioAudioSource.Stop();
            }
        }

        yield return new WaitForSeconds(radioSilenceDuration);

        if (radioAudioSource != null) radioAudioSource.Stop();

        if (enemyDummy != null) enemyDummy.SetActive(true);
        enemySpawned = true;

        if (enemyAudioSource != null)
        {
            if (sinisterMusic != null)
            {
                enemyAudioSource.clip = sinisterMusic;
                enemyAudioSource.Play();
            }

            if (voicelineSound != null)
            {
                enemyAudioSource.PlayOneShot(voicelineSound);
            }
        }
    }

    private void CheckIfPlayerIsLooking()
    {
        Vector3 directionToEnemy = (enemyDummy.transform.position - drivingCamera.transform.position).normalized;
        float angle = Vector3.Angle(drivingCamera.transform.forward, directionToEnemy);

        if (angle < lookAngleThreshold)
        {
            jumpscareTriggered = true;
            StartCoroutine(JumpscareSequence());
        }
    }

    private IEnumerator JumpscareSequence()
    {
        yield return new WaitForSeconds(stareTimeBeforeJumpscare);

        if (enemyAudioSource != null) enemyAudioSource.Stop();

        if (jumpscareSound != null && enemyAudioSource != null)
        {
            enemyAudioSource.PlayOneShot(jumpscareSound);
        }

        if (enemyAnimator != null) enemyAnimator.SetTrigger("Jumpscare");

        if (drivingCamera != null) drivingCamera.gameObject.SetActive(false);
        if (jumpscareCamera != null) jumpscareCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nextSceneName);
    }
}