using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject currentEquipedCannon;
    public Item currentEquipedCannonballItem;

    public GameObject equipedCannonballUi;
    public GameObject equipedCannonUi;

    public Inventory inventory;

    [SerializeField] private UIInventory uiInventory;
    public bool inventoryOpen;

    private void Start()
    {
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }

    public void AddItem(Item item)
    {
        inventory.AddItem(item);
    }
    public void RemoveItem(Item item)
    {
        inventory.RemoveItem(item);
    }
    // sets sprite, text, and currentEquipedCannonball
    public void EquipCannonball(Item item)
    {
        // just in case of bugs, unequip cannonball before equiping one
        UnequipCannonball();

        equipedCannonballUi.SetActive(true);
        currentEquipedCannonballItem = item;

        Image cannonballImage = equipedCannonballUi.GetComponent<Transform>().Find("cannonballImage").GetComponent<Image>();
        UpdateEquippedCannonballText(item.amount.ToString());
        cannonballImage.sprite = item.GetSprite();

        Debug.Log(currentEquipedCannon);
        currentEquipedCannon.GetComponent<Cannon>().canShoot = true;
    }

    // turns off cannonball ui object, and sets all text and sprites to null
    public void UnequipCannonball()
    {
        equipedCannonballUi.SetActive(false);
        currentEquipedCannonballItem = new Item { itemType = Item.ItemType.empty, amount = 0};
        currentEquipedCannonballItem = null;

        Image cannonballImage = equipedCannonballUi.GetComponent<Transform>().Find("cannonballImage").GetComponent<Image>();
        TextMeshProUGUI cannonballAmountText = equipedCannonballUi.GetComponent<Transform>().Find("text").GetComponent<TextMeshProUGUI>();
        UpdateEquippedCannonballText("");
        cannonballImage.sprite = null;
    }

    public void UpdateEquippedCannonballText(string text)
    {
        TextMeshProUGUI cannonballAmountText = equipedCannonballUi.GetComponent<Transform>().Find("text").GetComponent<TextMeshProUGUI>();
        cannonballAmountText.text = text;
    }

    private void Update()
    {
        // toggle inventory ui
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryOpen = !inventoryOpen;

            // turn inventory UI on or off based on inventoryEnabled VAR
            uiInventory.transform.Find("backgrounds").gameObject.SetActive(inventoryOpen);
            uiInventory.transform.Find("outlineSlots").gameObject.SetActive(inventoryOpen);
            foreach (GameObject go in uiInventory.itemSlots)
                go.transform.parent.gameObject.SetActive(inventoryOpen);
        }
    }
}
