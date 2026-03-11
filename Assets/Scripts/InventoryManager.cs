using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Slots")]
    public Image[] inventorySlots;
    public bool[] isFull;

    [Header("Hand Items")]
    // FIX: Removed the hardcoded [5] so Unity can handle whatever size we give it!
    public GameObject[] handItems;

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
        // FIX: Only check for keys 1, 2, and 3 now!
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
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
}