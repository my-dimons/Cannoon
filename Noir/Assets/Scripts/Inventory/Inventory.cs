using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged;
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        Debug.Log(itemList.Count);
    }

    public void AddItem(Item item)
    {
        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;

                    PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
                    if (playerManager.currentEquipedCannonballItem == null)
                    {

                    } else if (inventoryItem.itemType == playerManager.currentEquipedCannonballItem.itemType)
                    {
                        playerManager.UpdateEquippedCannonballText(inventoryItem.amount.ToString());
                    }
                }
            }
            if (!itemAlreadyInInventory)
                itemList.Add(new Item { itemType = item.itemType, amount = item.amount });
        }
        else
            itemList.Add(new Item { itemType = item.itemType, amount = item.amount });

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount -= item.amount;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(itemInInventory);
            }
        } else
        {
            itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
