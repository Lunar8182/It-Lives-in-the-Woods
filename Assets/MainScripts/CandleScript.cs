using UnityEngine;

public class CandleInteract : MonoBehaviour
{
    public int candleIndex;
    public CandlePuzzleManager puzzleManager;

    private bool isLit = false;

    public GameObject flameEffect;

    public void Interact()
    {
        if (isLit) return;

        isLit = true;

        if (flameEffect != null)
            flameEffect.SetActive(true);

        puzzleManager.RegisterCandle(candleIndex);
    }

    public void ResetCandle()
    {
        isLit = false;

        if (flameEffect != null)
            flameEffect.SetActive(false);
    }
}