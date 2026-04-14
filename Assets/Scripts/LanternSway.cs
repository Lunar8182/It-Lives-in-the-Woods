using UnityEngine;

public class LanternSway : MonoBehaviour
{
    public float swayMultiplier = 2f;
    public float swaySmoothness = 5f;
    public float maxSwayAmount = 5f;

    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float bobTimer = 0f;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * swayMultiplier;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Quaternion targetRotation = initialRotation * Quaternion.Euler(mouseY, -mouseX, 0);



        float moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        moveInput = Mathf.Clamp01(moveInput);

        if (moveInput > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
        }


        float waveY = Mathf.Sin(bobTimer) * bobAmount * moveInput;
        float waveX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * moveInput;

        Vector3 targetPosition = initialPosition + new Vector3(waveX, waveY, 0);


        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swaySmoothness);
    }
}