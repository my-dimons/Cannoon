using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Sprite selectedItemSlotImage;
    public Sprite unselectedItemSlotImage;
    public GameObject heldItem;
    public GameObject currentPlayerHeldObject;
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
        Item heldItemScript;
        // VARS
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player playerScript = player.GetComponent<Player>();
        SpriteRenderer playerImage = player.GetComponent<SpriteRenderer>();
        if (heldItem != null)
             heldItemScript = heldItem.GetComponent<Item>();
        else
            heldItemScript = null;
        // VARS

        // Deselect
        deselect.selected = false;
        deselect.GetComponent<Image>().sprite = unselectedItemSlotImage;
        Destroy(deselect.currentPlayerHeldObject);
        deselect.currentPlayerHeldObject = null;

        // Select
        selected = true;
        gameObject.GetComponent<Image>().sprite = selectedItemSlotImage;
        if (heldItem != null)
        {
            Debug.Log(heldItemScript.playerHoldingWeaponSprite);
            playerImage.sprite = heldItemScript.playerHoldingWeaponSprite;

            // spawn item in players hand
            currentPlayerHeldObject = Instantiate(heldItemScript.itemHoldingSprite, Vector3.zero, new Quaternion(0, 0, 0, 0), player.transform);
            currentPlayerHeldObject.transform.localPosition = heldItemScript.playerHeldSpriteLocation;
        } else
        {
            playerImage.sprite = playerScript.basePlayerSprite;
        }

        Debug.Log("Deselected Slot " + deselect.slot);
        Debug.Log("Selected Slot " + slot);
        return gameObject;
    }

    void ChangeSpriteImage()
    {
        Image slotImage = Instantiate(heldItem.GetComponent<Item>().itemSlotSprite, Vector3.zero, new Quaternion(0, 0, 0, 0), this.transform);
        slotImage.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }
}