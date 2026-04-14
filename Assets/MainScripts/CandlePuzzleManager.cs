using UnityEngine;
using System.Collections;

public class CandlePuzzleManager : MonoBehaviour
{
    public int[] correctOrder = { 0, 1, 2 };

    private int currentStep = 0;

    public CandleInteract[] candles;

    public DoorInteract door;

    public AudioSource successSound;
    public AudioSource failSound;

    public void RegisterCandle(int index)
    {
        if (index == correctOrder[currentStep])
        {
            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                PuzzleComplete();
            }
        }
        else
        {
            StartCoroutine(ResetPuzzle());
        }
    }

    void PuzzleComplete()
    {

        if (successSound != null)
            successSound.Play();

        door.ForceOpenFromPuzzle();
    }

    IEnumerator ResetPuzzle()
    {

        if (failSound != null)
            failSound.Play();

        yield return new WaitForSeconds(1f);

        foreach (CandleInteract candle in candles)
        {
            candle.ResetCandle();
        }

        currentStep = 0;
    }
}