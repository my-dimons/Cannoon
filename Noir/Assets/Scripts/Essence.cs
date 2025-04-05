using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Essence : MonoBehaviour
{
    [Tooltip("How much this essence is worth")]
    public float value;
    [Tooltip("Minimum distance the player needs to be for this essence to start to drift towards the player")]
    public float minPickupDistance;
    [Tooltip("How fast essence travels towards the player")]
    public float speed;

    public GameObject player;
    public TextMeshProUGUI essenceAmountText;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        essenceAmountText = GameObject.Find("Currency Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, this.gameObject.transform.position) < minPickupDistance)
        {
            transform.position = Vector2.MoveTowards(this.gameObject.transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    void Pickup()
    {
        gameManager.essence += value;
        essenceAmountText.text = gameManager.essence.ToString();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
        }
    }
}
