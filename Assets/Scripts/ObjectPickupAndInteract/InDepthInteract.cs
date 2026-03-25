using UnityEngine;
using System.Collections;

public class InDepthInteract : MonoBehaviour
{
    //This script is ONLY for objects that have in depth interaction and need more than one voice line
    //With screen text.

    public GameObject keyPrompt;
    public AudioClip voiceLine1;
    public AudioClip voiceLine2;

    public GameObject pressETextCar;
    public GameObject pressETextLock;

    public enum ItemType
    {
        Car,
        Lock
    }



    private bool voice1Played = false;
    private bool voice2Played = false;
    public ItemType itemType;
    private AudioSource voice;

    void Start()
    {
        voice = GetComponent<AudioSource>();
    }



    public void Interact()
    {
        if (voice.isPlaying) return;

        if (itemType == ItemType.Lock)
        {
            if (!voice1Played)
            {
                voice.PlayOneShot(voiceLine1);
                voice1Played = true;
            }
            else if (!voice2Played)
            {
                voice.PlayOneShot(voiceLine2);
                voice2Played = true;
            }
            else
            {
                StartCoroutine(ShowTextTemporaryLock());
            }
        }
        else if (itemType == ItemType.Car)
        {
            if (!voice1Played)
            {
                voice.PlayOneShot(voiceLine1);
                voice1Played = true;
            }
            else if (!voice2Played)
            {
                voice.PlayOneShot(voiceLine2);
                voice2Played = true;
            }
            else
            {
                StartCoroutine(ShowTextTemporaryCar());
            }
        }
    }




    IEnumerator ShowTextTemporaryCar()
    {
        pressETextCar.SetActive(true);

        yield return new WaitForSeconds(3f);

        pressETextCar.SetActive(false);
    }
    IEnumerator ShowTextTemporaryLock()
    {
        pressETextLock.SetActive(true);

        yield return new WaitForSeconds(3f);

        pressETextLock.SetActive(false);
    }
}