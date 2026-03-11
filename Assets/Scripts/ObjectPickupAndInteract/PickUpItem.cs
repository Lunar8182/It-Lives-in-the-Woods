using UnityEngine;

public class Interactable : MonoBehaviour
{
    //script for picking up any items. use the item type in unity to set the item type.
    //add it to the enum function so you can easily add more item types.
    public enum ItemType
    {
        Normal,
        MusicBox
    }

    public ItemType itemType;

    public GameObject objectToActivate;
    public GameObject keyPrompt;
    public GameObject playerMusic;

    private bool playerNearby = false;
    private bool hasMusicBox = false;

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

    void Interact()
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

        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().enabled = false;

        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;
    }

    void ToggleMusic()
    {
        AudioSource playerSource = playerMusic.GetComponent<AudioSource>();

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
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            keyPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            keyPrompt.SetActive(false);
        }
    }
}