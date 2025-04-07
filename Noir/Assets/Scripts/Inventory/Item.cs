using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        cannon,
        cannonball,
        bouncingCannonball
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.cannon: return ItemAssets.Instance.baseCannonSprite;
            case ItemType.cannonball: return ItemAssets.Instance.baseCannonballSprite;
            case ItemType.bouncingCannonball: return ItemAssets.Instance.bouncingCannonballSprite;
        }
    }
}
