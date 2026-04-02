using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Slots")]
    public Image[] inventorySlots;
    public bool[] isFull;

    [Header("Hand Items")]
    public GameObject[] handItems;
    public bool hasKey = false;
    public bool hasDoll = false;
    public bool hasRattle = false;
    public bool hasBlanket = false;
    public bool hasVoodooDoll = false;
    public bool hasWrench = false;
    [Header("Permanent Off-Hand Items")]
    public GameObject leftHandLantern;
    public bool hasLantern = false;

    private int selectedSlot = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        SelectSlot(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
    }

    public void SelectSlot(int index)
    {

        if (selectedSlot == index)
        {
            selectedSlot = -1;
        }
        else
        {
            selectedSlot = index;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i == selectedSlot)
            {
                inventorySlots[i].rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                inventorySlots[i].color = Color.white;
            }
            else
            {
                inventorySlots[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                inventorySlots[i].color = new Color(1, 1, 1, 0.5f);
            }

            if (i < handItems.Length && handItems[i] != null)
            {

                handItems[i].SetActive(i == selectedSlot);
            }
        }
    }

    public void AddItem(Sprite itemSprite, GameObject itemModel)
    {

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (isFull[i] == false)
            {
                isFull[i] = true;

                if (i < inventorySlots.Length)
                    inventorySlots[i].sprite = itemSprite;

                if (i < handItems.Length)
                {
                    handItems[i] = itemModel;

                    if (i == selectedSlot)
                    {
                        itemModel.SetActive(true);
                    }
                    else
                    {
                        itemModel.SetActive(false);
                    }
                }

                SelectSlot(selectedSlot);
                return;
            }
        }
    }
    public GameObject GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < handItems.Length)
        {
            return handItems[selectedSlot];
        }
        return null;
    }

    public void RemoveSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            inventorySlots[selectedSlot].sprite = null;
            inventorySlots[selectedSlot].color = new Color(1, 1, 1, 0.5f);
            isFull[selectedSlot] = false;

            // Clear the hand item
            if (handItems[selectedSlot] != null)
            {
                handItems[selectedSlot].SetActive(false);
                handItems[selectedSlot] = null;
            }
        }
    }

    public void EquipLantern()
    {
        hasLantern = true;
        if (leftHandLantern != null)
        {
            leftHandLantern.SetActive(true);
        }
    }
}