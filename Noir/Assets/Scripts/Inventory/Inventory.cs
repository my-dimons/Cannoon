using System;
using System.Collections.Generic;
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
            foreach(Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    Debug.Log(itemList);
                    //item.amount = 5;
                    //int amount = inventoryItem.amount;
                    //Debug.Log(inventoryItem.amount);
                    //Debug.Log(item.amount);
                    //Debug.Log(inventoryItem.amount + item.amount);
                    inventoryItem.amount += item.amount;
                    //inventoryItem.amount = amount;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                itemList.Add(new Item { itemType = item.itemType, amount = item.amount });
            }
        } else
            itemList.Add(new Item { itemType = item.itemType, amount = item.amount });
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
