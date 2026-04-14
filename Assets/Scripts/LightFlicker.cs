using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light flickerLight;

    public float minIntensity = 0.6f;
    public float maxIntensity = 1.2f;

    public float flickerSpeed = 0.1f;

    private float targetIntensity;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();

        SetNewTarget();
    }

    void Update()
    {
        flickerLight.intensity = Mathf.Lerp(flickerLight.intensity, targetIntensity, Time.deltaTime * 2f);

        if (Mathf.Abs(flickerLight.intensity - targetIntensity) < 0.05f)
        {
            Invoke(nameof(SetNewTarget), Random.Range(0.1f, 0.5f));
        }
    }

    void SetNewTarget()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }
}