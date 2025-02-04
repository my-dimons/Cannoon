using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Sprite selectedItemSlotImage;
    public Sprite unselectedItemSlotImage;
    public GameObject heldItem;
    public bool selected;
    public int slot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SelectItemSlot(ItemSlot deselect)
    {
        deselect.selected = false;
        deselect.GetComponent<Image>().sprite = unselectedItemSlotImage;
        selected = true;
        gameObject.GetComponent<Image>().sprite = selectedItemSlotImage;

        Debug.Log("Deselected Slot " + deselect.slot);
        Debug.Log("Selected Slot " + slot);
        return gameObject;
    }
}
