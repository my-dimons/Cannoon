using System.Collections;
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

    private Inventory inventory;

    [SerializeField] private UIInventory uiInventory;
    public bool inventoryEnabled;

    private void Start()
    {
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }

    private void Update()
    {
        // toggle inventory ui
        if (Input.GetKeyDown(KeyCode.E))
        {

            inventoryEnabled = !inventoryEnabled;
            
            if (inventoryEnabled)
            {
                uiInventory.transform.Find("background").gameObject.SetActive(false);
                foreach (GameObject go in uiInventory.itemSlots)
                    go.transform.parent.gameObject.SetActive(false);
            } else
            {
                uiInventory.transform.Find("background").gameObject.SetActive(true);
                foreach (GameObject go in uiInventory.itemSlots)
                    go.transform.parent.gameObject.SetActive(true);
            }
        }
    }
}
