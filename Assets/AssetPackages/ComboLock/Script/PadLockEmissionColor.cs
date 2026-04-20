// Script by Marcelli Michele - Modified for Solid Highlight instead of Blinking
using UnityEngine;

public class PadLockEmissionColor : MonoBehaviour
{
    private GameObject _myRuller;

    [HideInInspector]
    public bool _isSelect;

    [Header("Highlight Settings")]
    [Tooltip("The color the dial turns when selected")]
    public Color highlightColor = Color.yellow;

    [Tooltip("How bright the glow is. Keep it low so you can still read the numbers!")]
    [Range(0f, 2f)] public float glowIntensity = 0.4f;

    void Start()
    {
        _myRuller = gameObject;
    }

    public void BlinkingMaterial()
    {
        if (_myRuller == null) return;

        Material mat = _myRuller.GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        if (_isSelect)
        {
            mat.SetColor("_EmissionColor", highlightColor * glowIntensity);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.clear);
        }
    }
}