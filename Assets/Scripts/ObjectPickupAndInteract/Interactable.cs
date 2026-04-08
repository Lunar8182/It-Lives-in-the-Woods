using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

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
        Key,
        PrisonKey,
        VoodooDoll,
        Wrench
    }

    [Header("Ritual Settings")]
    public EnemyAI enemy;

    [Header("Blood Ritual Visuals")]
    public Volume bloodRitualVolume;
    public float fadeDuration = 5f;
    public AudioSource ritualMusicSource;
    public AudioClip bloodRitualClip;

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

    public GameObject alterDoll;
    public GameObject alterRattle;
    public GameObject alterBlanket;

    [Header("Altar Ritual Settings")]
    public GameObject voodooDollReward;
    public GameObject campsite;
    public GameObject ritualSite;
    public GameObject mapUpdateMessage;
    private bool placedDoll = false;
    private bool placedRattle = false;
    private bool placedBlanket = false;
    private bool ritualComplete = false;

    private bool hasMusicBox = false;
    private bool hasPlayedLanternAudio = false;
    private bool hasPlayedBabyRattleAudio = false;
    private bool poolOn = false;
    private bool portalOn = false;
    private bool alterOn = false;

    [Header("Music Box Settings")]
    public float stunRange = 5f;
    public AudioClip breakSound;

    void Update()
    {
        if (itemType == ItemType.MusicBox && Input.GetMouseButtonDown(0))
        {
            GameObject heldItem = InventoryManager.instance.GetSelectedItem();

            if (heldItem != null && heldItem == this.gameObject)
            {
                UseMusicBox();
            }
        }
    }

    public void UseMusicBox()
    {
        if (playerItem != null)
        {
            AudioSource playerSource = playerItem.GetComponent<AudioSource>();
            if (playerSource != null)
            {
                playerSource.Stop();
                playerSource.PlayOneShot(playerSource.clip);
            }
        }

        Vector3 playerPos = Camera.main.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(playerPos, 15f);
        bool enemyWasStunned = false;

        foreach (Collider col in hitColliders)
        {
            EnemyAI enemyScript = col.GetComponentInParent<EnemyAI>();
            if (enemyScript != null)
            {
                enemyScript.StunEnemy();
                enemyWasStunned = true;
                break;
            }
        }

        if (enemyWasStunned)
        {
            StartCoroutine(DelayedBreakSequence(playerPos));
        }
    }

    private IEnumerator DelayedBreakSequence(Vector3 soundPos)
    {
        yield return new WaitForSeconds(3f);

        if (breakSound != null)
        {
            AudioSource camSource = Camera.main.GetComponent<AudioSource>();
            if (camSource == null) camSource = Camera.main.gameObject.AddComponent<AudioSource>();

            camSource.PlayOneShot(breakSound);
            Debug.Log("Break sound should be playing now!");
        }
        else
        {
            Debug.LogError("The breakSound clip is EMPTY in the Inspector!");
        }

        InventoryManager.instance.RemoveSelectedItem();

        if (objectToActivate != null) objectToActivate.SetActive(false);

        yield return null;
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (letterInteract != null && letterInteract.isReading)
        {
            if (keyPrompt != null) keyPrompt.SetActive(false);
        }
    }

    public void Interact()
    {

        if (itemType == ItemType.Letter)
        {
            if (keyPrompt != null)
            {
                letterInteract.InteractPaper();
                return;
            }

        }
        CandleInteract candle = GetComponent<CandleInteract>();

        if (candle != null)
        {
            candle.Interact();
            return;
        }

        if (itemType == ItemType.Doll)
        {
            InventoryManager.instance.hasDoll = true;
        }
        if (itemType == ItemType.BabyRattle)
        {
            InventoryManager.instance.hasRattle = true;
        }
        if (itemType == ItemType.Blanket)
        {
            InventoryManager.instance.hasBlanket = true;
        }

        if (itemType == ItemType.VoodooDoll)
        {
            InventoryManager.instance.hasVoodooDoll = true;
        }

        if (itemType == ItemType.Key)
        {
            InventoryManager.instance.hasKey = true;
        }

        if (itemType == ItemType.PrisonKey)
        {
            InventoryManager.instance.hasPrisonKey = true;
        }

        if (itemType == ItemType.Wrench)
        {
            InventoryManager.instance.hasWrench = true;
        }

        if (itemType == ItemType.MusicBox)
        {
            hasMusicBox = true;
        }

        if (keyPrompt != null)
        {
            keyPrompt.SetActive(false);
        }

        if (itemType == ItemType.Lantern)
        {
            InventoryManager.instance.EquipLantern();

            gameObject.SetActive(false);

            return;
        }

        if (itemType == ItemType.Altar)
        {
            if (!alterOn)
            {
                altar.ToggleDemonicAltar();
                alterOn = true;
                if (keyPrompt != null) keyPrompt.SetActive(false);
                return;
            }

            if (alterOn && !ritualComplete)
            {
                GameObject itemInHand = InventoryManager.instance.GetSelectedItem();

                if (itemInHand != null)
                {
                    if (InventoryManager.instance.hasDoll && itemInHand.name.Contains("Doll"))
                    {
                        alterDoll.SetActive(true);
                        InventoryManager.instance.hasDoll = false;
                        InventoryManager.instance.RemoveSelectedItem();
                        placedDoll = true;
                    }
                    else if (InventoryManager.instance.hasRattle && itemInHand.name.Contains("Rattle"))
                    {
                        alterRattle.SetActive(true);
                        InventoryManager.instance.hasRattle = false;
                        InventoryManager.instance.RemoveSelectedItem();
                        placedRattle = true;
                    }
                    else if (InventoryManager.instance.hasBlanket && itemInHand.name.Contains("Blanket"))
                    {
                        alterBlanket.SetActive(true);
                        InventoryManager.instance.hasBlanket = false;
                        InventoryManager.instance.RemoveSelectedItem();
                        placedBlanket = true;
                    }



                    if (placedDoll && placedRattle && placedBlanket)
                    {
                        ritualComplete = true;


                        if (voodooDollReward != null)
                        {
                            voodooDollReward.SetActive(true);
                        }
                        if (campsite != null)
                        {
                            campsite.SetActive(false);
                        }
                        if (ritualSite != null)
                        {
                            ritualSite.SetActive(true);
                        }
                        if (bloodRitualVolume != null)
                        {
                            StartCoroutine(FadeInBloodRitual());
                        }

                        StartCoroutine(ShowMapMessage());

                    }
                }
            }
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

        gameObject.SetActive(false);
    }

    private IEnumerator FadeInBloodRitual()
    {
        float elapsed = 0;

        if (ritualMusicSource != null && bloodRitualClip != null)
        {
            ritualMusicSource.clip = bloodRitualClip;
            ritualMusicSource.volume = 0.5f;
            ritualMusicSource.loop = true;
            ritualMusicSource.Play();
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bloodRitualVolume.weight = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            yield return null;
        }

        if (enemy != null)
        {
            enemy.TriggerEnragedState();
        }

        bloodRitualVolume.weight = 1f;
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

    private System.Collections.IEnumerator ShowMapMessage()
    {
        if (mapUpdateMessage != null)
            mapUpdateMessage.SetActive(true);

        yield return new WaitForSeconds(10f);

        if (mapUpdateMessage != null)
            mapUpdateMessage.SetActive(false);
    }

}