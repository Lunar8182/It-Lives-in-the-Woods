using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float drainRate = 20f;
    public float regenRate = 15f;

    [Header("UI")]
    public Image staminaFill;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += regenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentStamina / maxStamina;
            Debug.Log("Fill amount = " + staminaFill.fillAmount);
        }
        else
        {
            Debug.Log("staminaFill is NULL");
        }
    }
}