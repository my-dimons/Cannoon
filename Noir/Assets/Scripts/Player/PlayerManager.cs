using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject currentEquipedCannon;
    public GameObject currentEquipedCannonball;

    [Tooltip("All bought cannons currently in the inventory")]
    public List<GameObject> boughtCannons;

    [Tooltip("All bought cannonballs currently in the inventory (must have >0 cannonballs in stack)")]
    public List<GameObject> boughtCannonballs;

    public Inventory inventory;

    [SerializeField] private UIInventory uiInventory;
    public bool inventoryEnabled;

    private void Start()
    {
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }

    public void AddItem(Item item)
    {
        inventory.AddItem(item);
    }

    private void Update()
    {
        // toggle inventory ui
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryEnabled = !inventoryEnabled;

            // turn inventory UI on or off based on inventoryEnabled VAR
            uiInventory.transform.Find("backgrounds").gameObject.SetActive(inventoryEnabled);
            uiInventory.transform.Find("outlineSlots").gameObject.SetActive(inventoryEnabled);
            foreach (GameObject go in uiInventory.itemSlots)
                go.transform.parent.gameObject.SetActive(inventoryEnabled);
        }
    }
}
