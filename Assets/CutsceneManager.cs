using UnityEngine;
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
    public AudioClip introMusic;        // The 10-second country song
    public AudioClip radioSilenceSound; // The static/silence sound
    public AudioClip sinisterMusic;     // The scary music that starts when they spawn

    [Header("References")]
    public GameObject enemyDummy;
    public Animator enemyAnimator;

    [Header("Cameras")]
    public Camera drivingCamera;
    public Camera jumpscareCamera;

    [Header("Audio Sources (Drag these in!)")]
    [Tooltip("The AudioSource physically attached to your Radio object")]
    public AudioSource radioAudioSource;
    [Tooltip("The AudioSource attached to this Cutscene Manager or the Enemy")]
    public AudioSource enemyAudioSource;

    [Header("Enemy Audio SFX")]
    public AudioClip voicelineSound;    // The enemy laughing
    public AudioClip jumpscareSound;

    [Header("End Scene")]
    public string nextSceneName = "GameOverScene";

    private bool enemySpawned = false;
    private bool jumpscareTriggered = false;

    void Start()
    {
        if (enemyDummy != null) enemyDummy.SetActive(false);
        if (jumpscareCamera != null) jumpscareCamera.gameObject.SetActive(false);

        // If you forget to attach an enemy audio source, it will make one for you automatically
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
        // --- PHASE 1: The Country Song (Plays from the Radio) ---
        if (radioAudioSource != null && introMusic != null)
        {
            radioAudioSource.clip = introMusic;
            radioAudioSource.Play();
        }

        yield return new WaitForSeconds(introMusicDuration);

        // --- PHASE 2: Radio Silence (Plays from the Radio) ---
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

        // --- PHASE 3: The Reveal (Plays from the Enemy) ---

        // FIX 1: Explicitly shut off the radio so the static stops looping!
        if (radioAudioSource != null) radioAudioSource.Stop();

        // FIX 2: Wake up the Enemy Dummy FIRST, before asking it to make noise!
        if (enemyDummy != null) enemyDummy.SetActive(true);
        enemySpawned = true;

        // NOW play the scary audio, since the enemy is actually awake to broadcast it
        if (enemyAudioSource != null)
        {
            // Start the scary background music
            if (sinisterMusic != null)
            {
                enemyAudioSource.clip = sinisterMusic;
                enemyAudioSource.Play();
            }

            // Play the laughing voice line over the top of the scary music
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

        // Stop the sinister background music right as the jumpscare happens
        if (enemyAudioSource != null) enemyAudioSource.Stop();

        // Play the loud scream!
        if (jumpscareSound != null && enemyAudioSource != null)
        {
            enemyAudioSource.PlayOneShot(jumpscareSound);
        }

        if (enemyAnimator != null) enemyAnimator.SetTrigger("Jumpscare");

        if (drivingCamera != null) drivingCamera.gameObject.SetActive(false);
        if (jumpscareCamera != null) jumpscareCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        // --- END SCENE ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nextSceneName);
    }
}