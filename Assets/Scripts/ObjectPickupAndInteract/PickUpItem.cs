using UnityEngine;

public class Interactable : MonoBehaviour
{
    //script for picking up any items. use the item type in unity to set the item type.
    //add it to the enum function so you can easily add more item types.
    public enum ItemType
    {
        Normal,
        MusicBox,
        Lantern,
        BabyRattle,
        Doll,
        Blanket,

    }

    public ItemType itemType;
    //object to activate is the object that will be activated on the screen.
    public GameObject objectToActivate;
    public GameObject keyPrompt;
    //Use this if you want to play music or a sound from an item.
    //Add the object that has the audio source attached (usually the same as object to activate) 
    public GameObject playerItem;

    private bool playerNearby = false;
    private bool hasMusicBox = false;
    private bool hasPlayedLanternAudio = false;

    private bool hasPlayedBabyRattleAudio = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (hasMusicBox && itemType == ItemType.MusicBox && Input.GetMouseButtonDown(0))
        {
            ToggleMusic();
        }
    }

    public void Interact()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (itemType == ItemType.MusicBox)
        {
            hasMusicBox = true;
        }

        if (keyPrompt != null)
        {
            keyPrompt.SetActive(false);
        }

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            r.enabled = false;
        }

        Light[] allLights = GetComponentsInChildren<Light>();
        foreach (Light light in allLights)
        {
            light.enabled = false;
        }

        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    public void ToggleMusic()
    {
        AudioSource playerSource = playerItem.GetComponent<AudioSource>();

        if (playerSource.isPlaying)
        {
            playerSource.Pause();
        }
        else
        {
            playerSource.Play();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (itemType == ItemType.Lantern && hasPlayedLanternAudio == false)
        {
            hasPlayedLanternAudio = true;
            AudioSource lanternAudio = GetComponent<AudioSource>();
            lanternAudio.Play();
        }
        else if (itemType == ItemType.BabyRattle && hasPlayedBabyRattleAudio == false)
        {
            hasPlayedBabyRattleAudio = true;
            AudioSource babyRattleAudio = GetComponent<AudioSource>();
            babyRattleAudio.Play();
        }
    }



}