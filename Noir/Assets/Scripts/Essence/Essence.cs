using System.Collections;
using UnityEngine;

public class Essence : MonoBehaviour
{
    [Tooltip("How much this essence is worth")]
    public int value;
    [Tooltip("Minimum distance the player needs to be for this essence to start to drift towards the player")]
    public float minPickupDistance;
    [Tooltip("How fast essence travels towards the player")]
    public float speed;
    [Tooltip("Can this be essence follow the player?")]

    public bool canFollowPlayer;
    [Tooltip("Follow player delay upon spawning (In Seconds)")]
    public float playerFollowDelay;

    GameObject player;
    EssenceManager essenceManager;

    IEnumerator PlayerFollowDelayTimer()
    {
        canFollowPlayer = false;
        yield return new WaitForSeconds(playerFollowDelay);
        canFollowPlayer = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        essenceManager = GameObject.FindGameObjectWithTag("EssenceManager").GetComponent<EssenceManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);

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
        essenceManager.essence += value;

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
