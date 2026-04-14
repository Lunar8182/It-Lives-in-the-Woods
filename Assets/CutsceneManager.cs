using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CarCutsceneManager : MonoBehaviour
{
    public float timeUntilSpawn = 20f;
    public float stareTimeBeforeJumpscare = 3f;

    public float lookAngleThreshold = 45f;

    public GameObject enemyDummy;
    public Animator enemyAnimator;

    [Header("Cameras")]
    public Camera drivingCamera;
    public Camera jumpscareCamera;

    public AudioSource audioSource;
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
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

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
        yield return new WaitForSeconds(timeUntilSpawn);

        if (enemyDummy != null) enemyDummy.SetActive(true);
        if (voicelineSound != null) audioSource.PlayOneShot(voicelineSound);

        enemySpawned = true;
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

        if (jumpscareSound != null) audioSource.PlayOneShot(jumpscareSound);
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