using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Slots")]
    // Make sure these are the child "Icon" images, not the backgrounds!
    public Image[] inventorySlots;

    // NOTE: Removed 'isFull' array because checking for an empty sprite is much safer!

    [Header("Hand Items")]
    public GameObject[] handItems;
    public bool hasKey = false;
    public bool hasPrisonKey = false;
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
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == null)
            {
                inventorySlots[i].color = new Color(1, 1, 1, 0f); // 0f = perfectly invisible
            }
        }

        SelectSlot(0);
    }

    private void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
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
            // Only adjust visibility/colors if there is actually an item in this slot!
            if (inventorySlots[i].sprite != null)
            {
                if (i == selectedSlot)
                {
                    inventorySlots[i].rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    inventorySlots[i].color = new Color(1, 1, 1, 1f); // 1f = fully visible
                }
                else
                {
                    inventorySlots[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                    inventorySlots[i].color = new Color(1, 1, 1, 0.5f); // 0.5f = slightly faded out
                }
            }
            else
            {
                // Keep empty slots invisible so they don't look like white boxes
                inventorySlots[i].color = new Color(1, 1, 1, 0f);
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
            if (inventorySlots[i].sprite == null)
            {
                inventorySlots[i].sprite = itemSprite;
                inventorySlots[i].preserveAspect = true;

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

            // Make the slot completely invisible again now that it's empty
            inventorySlots[selectedSlot].color = new Color(1, 1, 1, 0f);

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