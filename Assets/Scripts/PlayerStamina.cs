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

    void Start()
    {
        // Initialize speed
        currentMoveSpeed = normalSpeed;
    }

    void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        // 1. Sprinting & Draining
        // Only works if holding shift, NOT exhausted, and has stamina
        if (isHoldingShift && !isExhausted && currentStamina > 0)
        {
            currentStamina -= drainRate * Time.deltaTime;
            currentMoveSpeed = sprintSpeed;

            // Trigger exhaustion if stamina dips to 5 or below
            if (currentStamina <= 5f)
            {
                isExhausted = true;
            }
        }
        else
        {
            // 2. Regeneration
            // Only refill if the player has completely let go of the Shift key
            if (!isHoldingShift)
            {
                currentStamina += regenRate * Time.deltaTime;
            }

            // 3. State Management (Exhausted vs Normal)
            if (isExhausted)
            {
                currentMoveSpeed = exhaustedSpeed; // Force slow speed

                // Recover once stamina reaches 50% of max
                if (currentStamina >= maxStamina * 0.5f)
                {
                    isExhausted = false;
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