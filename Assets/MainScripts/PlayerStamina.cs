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
    public float exhaustedSpeed = 2.5f;
    public float currentMoveSpeed;

    [Header("UI")]
    public Image staminaFill;

    private bool isExhausted = false;
    private bool requireShiftRelease = false;

    void Start()
    {
        // Initialize speed
        currentMoveSpeed = normalSpeed;
    }

    void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        if (!isHoldingShift)
        {
            requireShiftRelease = false;
        }

        bool isSprinting = isHoldingShift && !isExhausted && currentStamina > 0 && !requireShiftRelease;

        if (isSprinting)
        {
            currentStamina -= drainRate * Time.deltaTime;
            currentMoveSpeed = sprintSpeed;

            if (currentStamina <= 5f)
            {
                isExhausted = true;
                requireShiftRelease = true;
            }
        }
        else
        {

            currentStamina += regenRate * Time.deltaTime;

            if (isExhausted)
            {
                currentMoveSpeed = exhaustedSpeed;

                if (currentStamina >= maxStamina * 0.5f)
                {
                    isExhausted = false;
                }
            }
            else
            {
                currentMoveSpeed = normalSpeed;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentStamina / maxStamina;
            staminaFill.color = isExhausted ? Color.red : Color.white;
        }
    }
}