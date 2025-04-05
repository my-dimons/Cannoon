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
    [Tooltip("Can this be essence follow the player?")]

    public bool canFollowPlayer;
    [Tooltip("Follow player delay upon spawning (In Seconds)")]
    public float playerFollowDelay;

    public GameObject player;
    public TextMeshProUGUI essenceAmountText;
    GameManager gameManager;

    IEnumerator PlayerFollowDelayTimer()
    {
        canFollowPlayer = false;
        yield return new WaitForSeconds(playerFollowDelay);
        canFollowPlayer = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        essenceAmountText = GameObject.Find("Currency Text").GetComponent<TextMeshProUGUI>();

        StartCoroutine(PlayerFollowDelayTimer());
    }

    private void Update()
    {
        FollowPlayer();
    }

    // Moves towards the player for easy collection
    private void FollowPlayer()
    {
        if (Vector2.Distance(player.transform.position, this.gameObject.transform.position) < minPickupDistance && canFollowPlayer)
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
