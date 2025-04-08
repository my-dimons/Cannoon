using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("Buy Buttons")]
    public GameObject[] cannonBuyButtons;
    public GameObject[] cannonballBuyButtons;

    [Header("Buyable Items")]
    [Tooltip("Cannons that can be sold in this shop, a cannon and its price is randomly selected")]
    public List<Item.ItemType> buyableCannons;
    [Tooltip("cannonballs that can be sold in this shop, a cannonballs price and amount is randomly selected")]
    public List<Item.ItemType> buyableCannonballs;

    [Header("Buy Value")]
    public int minCannonPrice;
    public int maxCannonPrice;


    public int minCannonballPrice;
    public int maxCannonballPrice;

    public int minCannonballAmount;
    public int maxCannonballAmount;

    [Header("Referances")]
    [SerializeField] private EssenceManager essenceManager;
    [SerializeField] private ItemAssets itemAssets;
    [SerializeField] private PlayerManager playerManager;
    private Inventory inventory;

    private void Start()
    {
        foreach (GameObject button in cannonBuyButtons)
        {

        }
        foreach (GameObject button in cannonballBuyButtons)
        {
            Transform transform = button.GetComponent<Transform>();
            // get random cannonball, cannonball amount, and price
            Item.ItemType type = buyableCannonballs[Random.Range(0, buyableCannonballs.Count)];
            Debug.Log(type);
            int amount = Random.Range(minCannonballAmount, maxCannonballAmount);
            int price = Random.Range(minCannonballPrice, maxCannonballPrice);

            // change sprite, and amount text
            Item item = new Item { itemType = type, amount = amount };
            Image image = transform.Find("Canvas").Find("cannonballImage").GetComponent<Image>();
            TextMeshProUGUI text = transform.Find("Canvas").Find("text").GetComponent<TextMeshProUGUI>();
            image.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            image.sprite = item.GetSprite();
            text.text = "x" + amount.ToString();

            // add onclick listenter
            Button buyButton = transform.Find("Canvas").Find("buyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyCannonball(item, price));

            // show price on text
            TextMeshPro priceText = transform.Find("priceText").GetComponent<TextMeshPro>();
            priceText.text = price.ToString();
        }
    }

    public void BuyCannon(GameObject cannon, int price)
    {
        essenceManager.SpendEssence(price);
    }

    public void BuyCannonball(Item cannonball, int price)
    {
        if (essenceManager.essence >= price)
        {
            Debug.Log(cannonball.amount);
            playerManager.AddItem(cannonball);
            essenceManager.SpendEssence(price);
        } else
        {
            Debug.Log("Not Enough Essence");
        }
    }
}
