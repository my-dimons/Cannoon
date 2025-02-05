using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Melee,
        Gun,
        Bow
    }
    public ItemType type;
    [Header("General")]
    public Image itemSlotSprite;
    public GameObject itemHoldingSprite;
    public Sprite playerHoldingWeaponSprite;
    public string itemName;

    public Vector3 playerHeldSpriteLocation;

    [Header("For Gun")]
    public Gun gunScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
