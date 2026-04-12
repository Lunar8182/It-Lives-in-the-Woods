using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LockpickQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform indicator;
    public RectTransform targetArea;
    public RectTransform barBackground;
    public TextMeshProUGUI progressText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;

    [Header("Collider Hitboxes")]
    public BoxCollider2D indicatorCollider;
    public BoxCollider2D targetCollider;

    [Header("Settings")]
    public float baseMoveSpeed = 500f;
    public int successesNeeded = 3;
    public float targetWidth = 30f;

    private float currentMoveSpeed;
    private float barWidth;
    private int currentSuccesses = 0;
    private bool movingRight = true;
    private bool isActive = false;

    [Header("Events")]
    public UnityEvent onGameWin;
    public UnityEvent onGameExit;

    void Start()
    {
        barWidth = barBackground.rect.width;
        targetArea.sizeDelta = new Vector2(targetWidth, targetArea.sizeDelta.y);
        minigamePanel.SetActive(false);
    }

    public void StartGame()
    {
        isActive = true;
        currentSuccesses = 0;
        currentMoveSpeed = baseMoveSpeed;
        UpdateUI();
        RandomizeTarget();
        minigamePanel.SetActive(true);
    }

    void Update()
    {
        if (!isActive) return;

        MoveIndicator();

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelGame();
        }
    }

    void MoveIndicator()
    {
        float halfBar = barWidth / 2f;
        float currentX = indicator.anchoredPosition.x;

        if (currentX >= halfBar) movingRight = false;
        else if (currentX <= -halfBar) movingRight = true;

        float move = (movingRight ? 1 : -1) * currentMoveSpeed * Time.deltaTime;
        indicator.anchoredPosition += new Vector2(move, 0);
    }

    void RandomizeTarget()
    {
        float limit = (barWidth / 2f) - (targetWidth / 2f);
        float randomX = Random.Range(-limit, limit);
        targetArea.anchoredPosition = new Vector2(randomX, 0);
    }

    void CheckClick()
    {
        Physics2D.SyncTransforms();

        if (indicatorCollider.bounds.Intersects(targetCollider.bounds))
        {
            Debug.Log("<color=green>HIT! The colliders are touching!</color>");

            if (audioSource != null && successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
            
            currentSuccesses++;

            if (currentSuccesses >= successesNeeded)
            {
                Win();
            }
            else
            {
                UpdateUI();
                RandomizeTarget();
                currentMoveSpeed *= 1.25f;
            }
        }
        else
        {
            Debug.Log("<color=red>MISS! The colliders are not touching.</color>");
            Fail();
        }
    }

    void UpdateUI()
    {
        if (progressText != null)
            progressText.text = $"Locks: {currentSuccesses}/{successesNeeded}";
    }

    void Win()
    {
        isActive = false;
        minigamePanel.SetActive(false);
        onGameWin.Invoke();
    }

    void Fail()
    {
        currentSuccesses = 0;
        currentMoveSpeed = baseMoveSpeed;
        UpdateUI();
        RandomizeTarget();
    }

    public void CancelGame()
    {
        isActive = false;
        minigamePanel.SetActive(false);
        onGameExit.Invoke();
        ResetPlayer();
    }

    void ResetPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}