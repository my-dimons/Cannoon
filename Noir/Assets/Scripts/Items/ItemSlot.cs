using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
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

    public void SelectItemSlot(ItemSlot deselect)
    {
        deselect.selected = false;
        selected = true;
        Debug.Log("Deselected Slot " + deselect.slot);
        Debug.Log("Selected Slot " + slot);
    }
}
