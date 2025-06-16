using Pathfinding;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour // really just flybound ai, but im too lazy to change the name
{
    public Transform target;
    public Transform enemySprite;

    public float nextWaypointDistance = 3f;
    public float hoveringHeight;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    // OTHER: Referenced in Start()
    Seeker seeker;
    Rigidbody2D rb;
    Enemy enemyScript;
    EndlessMode endlessModeScript;
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        enemyScript = GetComponent<Enemy>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(GetTarget());
        IEnumerator GetTarget()
        {
            yield return new WaitForSeconds(.01f);

            target = this.gameObject.GetComponent<Enemy>().target;
            InvokeRepeating(nameof(UpdatePath), 0f, .4f);
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(new(rb.position.x, rb.position.y + 1, 0), new(target.position.x, target.position.y + hoveringHeight, 0), OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        // Move towards the next waypoint (player)
        if (enemyScript.canMove && !enemyScript.frozen)
            rb.AddForce(direction * enemyScript.speed, ForceMode2D.Impulse);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        FlipSprite();
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    // Deal damage to player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions"))
        {
            Debug.Log("Dealing damage to player");
            target.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

    void FlipSprite()
    {
        // flips the enemy to face the proper direction
        if (target.transform.position.x >= transform.position.x && enemyScript.canTurn)
        {
            enemySprite.localScale = new Vector3(-1f, 1f, 1f);
            enemyScript.facingRight = true;
        }
        else if (target.transform.position.x < transform.position.x && enemyScript.canTurn)
        {
            enemySprite.localScale = new Vector3(1f, 1f, 1f);
            enemyScript.facingRight = false;
        }
        else if (target.transform.position.x == transform.position.x)
            enemySprite.localScale = enemySprite.localScale;
    }
}
