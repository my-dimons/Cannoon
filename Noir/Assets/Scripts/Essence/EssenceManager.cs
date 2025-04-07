using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// MANAGES ESSENCE CURRENCY, buying, spending, and gaining essence is managed here
public class EssenceManager : MonoBehaviour
{
    [Tooltip("How much essence the player has")]
    public int essence;
    public TextMeshProUGUI essenceAmountText;
    PlayerManager playerManager;

    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }
    private void Update()
    {
        essenceAmountText.text = essence.ToString();
    }

    public void SpendEssence(int price)
    {
        essence -= price;
    }
}
