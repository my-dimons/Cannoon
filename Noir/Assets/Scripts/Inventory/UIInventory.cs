using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    public List<GameObject> itemSlots;

    public PlayerManager playerManager;
    private void Start()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }
    public void SetInventory(Inventory inventory) {
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 250f;
        itemSlots.Clear();

        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(itemSlotTemplate.GetComponent<RectTransform>().anchoredPosition.x + x * itemSlotCellSize,
                itemSlotTemplate.GetComponent<RectTransform>().anchoredPosition.y + y *  itemSlotCellSize);
            Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
            image.sprite = item.GetSprite();
            x++;
            if (x > 5)
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
