using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float drainRate = 20f;
    public float regenRate = 15f;

    [Header("Speed Settings")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 10f;
    public float exhaustedSpeed = 2.5f; // Half of normal speed
    public float currentMoveSpeed;

    [Header("UI")]
    public Image staminaFill;

    private bool isExhausted = false;
    // NEW: Tracks if the player needs to let go of Shift to sprint again
    private bool requireShiftRelease = false;

    void Start()
    {
        // Initialize speed
        currentMoveSpeed = normalSpeed;
    }

    void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        // Reset the requirement to release shift if the player lets go of the key
        if (!isHoldingShift)
        {
            requireShiftRelease = false;
        }

        // Determine if the player should actively be sprinting right now
        bool isSprinting = isHoldingShift && !isExhausted && currentStamina > 0 && !requireShiftRelease;

        // 1. Sprinting & Draining
        if (isSprinting)
        {
            currentStamina -= drainRate * Time.deltaTime;
            currentMoveSpeed = sprintSpeed;

            // Trigger exhaustion if stamina dips to 5 or below
            if (currentStamina <= 5f)
            {
                isExhausted = true;
                requireShiftRelease = true; // Force them to release shift to sprint again
            }
        }
        else
        {
            // 2. Regeneration
            // This now happens anytime the player isn't actively sprinting
            currentStamina += regenRate * Time.deltaTime;

            // 3. State Management (Exhausted vs Normal)
            if (isExhausted)
            {
                currentMoveSpeed = exhaustedSpeed; // Force slow speed

                // Recover once stamina reaches 50% of max
                if (currentStamina >= maxStamina * 0.5f)
                {
                    isExhausted = false;
                    // Note: requireShiftRelease stays true until they physically let go of Shift
                }
            }
            else
            {
                currentMoveSpeed = normalSpeed; // Back to normal walking
            }
        }

        // 4. Clamp & UI Update
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentStamina / maxStamina;
            // Turn the bar red when exhausted, white when normal
            staminaFill.color = isExhausted ? Color.red : Color.white;
        }
    }
}