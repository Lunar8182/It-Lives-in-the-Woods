using TMPro;
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
        Altar,
        Portal,
        Telescope,
        Pool,
        Letter,
        Key





    }

    [Header("Inventory Settings")]
    public Sprite itemIcon;

    public ItemType itemType;
    //object to activate is the object that will be activated on the screen.
    public GameObject objectToActivate;
    public GameObject keyPrompt;
    //Use this if you want to play music or a sound from an item.
    //Add the object that has the audio source attached (usually the same as object to activate) 
    public GameObject playerItem;
    public DemonicAltar_Controller altar;
    public BloodPool_Controller pool;
    public HellGate_Controller hellGate;
    public LetterInteract letterInteract;

    private bool hasMusicBox = false;
    private bool hasPlayedLanternAudio = false;
    private bool hasPlayedBabyRattleAudio = false;
    private bool poolOn = false;
    private bool portalOn = false;
    private bool alterOn = false;

    void Update()
    {
        if (hasMusicBox && itemType == ItemType.MusicBox && Input.GetMouseButtonDown(0))
        {
            ToggleMusic();
        }
    }

    public void Interact()
    {

        if (itemType == ItemType.Letter)
        {
            if (keyPrompt != null)
            {
                letterInteract.InteractPaper();
                keyPrompt.SetActive(false);
                return;
            }

        }
        if (itemType == ItemType.Key)
        {
            InventoryManager.instance.hasKey = true;
            Debug.Log("Key obtained!");
        }

        if (itemType == ItemType.MusicBox)
        {
            hasMusicBox = true;
        }

        if (keyPrompt != null)
        {
            keyPrompt.SetActive(false);
        }

        if (itemType == ItemType.Altar)
        {
            if (alterOn) return;
            altar.ToggleDemonicAltar();
            alterOn = true;
            keyPrompt.SetActive(false);
            return;
        }

        if (itemType == ItemType.Portal)
        {
            if (portalOn) return;
            hellGate.ToggleHellGate();
            portalOn = true;
            keyPrompt.SetActive(false);
            return;
        }

        if (itemType == ItemType.Telescope)
        {
            TelescopeController telescope = GetComponentInParent<TelescopeController>();
            telescope.UseTelescope();
            return;
        }

        if (itemType == ItemType.Pool)
        {
            if (poolOn) return;
            pool.F_ToggleBloodPool();
            poolOn = true;
            keyPrompt.SetActive(false);
            return;
        }

        if (itemIcon != null && objectToActivate != null)
        {
            InventoryManager.instance.AddItem(itemIcon, objectToActivate);
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
        if (!other.CompareTag("Player"))
        {
            return;
        }

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