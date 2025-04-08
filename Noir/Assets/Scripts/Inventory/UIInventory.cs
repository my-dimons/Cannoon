using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    public List<GameObject> itemSlots;

    public PlayerManager playerManager;

    [Header("Inventory Slot Cells")]
    [Tooltip("Maximum slots along the x axis")]
    public int maxX;
    [Tooltip("Maximum slots along the y axis")]
    public int maxY;
    [Tooltip("Space between item slots")]
    public float itemSlotCellSize;

    private void Start()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }
    public void SetInventory(Inventory inventory) {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
        int x = 0;
        int y = 0;
        itemSlots.Clear();

        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(itemSlotTemplate.GetComponent<RectTransform>().anchoredPosition.x + x * itemSlotCellSize,
                itemSlotTemplate.GetComponent<RectTransform>().anchoredPosition.y + -y * itemSlotCellSize);

            Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            TextMeshProUGUI uiText = itemSlotRectTransform.Find("text").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
                uiText.SetText(item.amount.ToString());
            else
                uiText.SetText("");

            x++;
            if (x >= maxX)
            {
                x = 0;
                y++;
            }

            itemSlots.Add(image.gameObject);

            if (!playerManager.inventoryEnabled)
            {
                image.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
