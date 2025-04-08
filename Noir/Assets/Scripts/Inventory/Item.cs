using UnityEngine;

public class Item
{
    public enum ItemType
    {
        baseCannon,
        baseCannonball,
        bouncingCannonball
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            case ItemType.baseCannon:           return ItemAssets.Instance.baseCannonSprite;
            case ItemType.baseCannonball:       return ItemAssets.Instance.baseCannonballSprite;
            case ItemType.bouncingCannonball:   return ItemAssets.Instance.bouncingCannonballSprite;

            default: return null;
        }
    }

    public bool IsStackable()
    {
        switch(itemType)
        {
            default:
            case ItemType.baseCannonball:
            case ItemType.bouncingCannonball:
                return true;
            case ItemType.baseCannon:
                return false;
        }
    }
}
