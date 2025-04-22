using TMPro;
using UnityEngine;

// MANAGES ESSENCE CURRENCY, buying, spending, and gaining essence is managed here
public class EssenceManager : MonoBehaviour
{
    [Tooltip("How much essence the player has")]
    public int essence;
    public TextMeshProUGUI essenceAmountText;

    private void Update()
    {
        essenceAmountText.text = essence.ToString();
    }

    public void SpendEssence(int price)
    {
        essence -= price;
    }
}
