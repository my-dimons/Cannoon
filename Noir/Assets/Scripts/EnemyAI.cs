using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;

    public float speed;

    public float jumpForce;
    public float jumpThreshold;
    public float jumpCooldown;
    public float jumpCheckDistance;
    public LayerMask layerMask;

    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
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
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float forceX = direction.x * speed * Time.deltaTime;

        rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);

        //Jump
        CheckJump();
        if (target.position.y > rb.position.y + jumpThreshold)
        {
            Jump(forceX);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (forceX >= 0.01f)
            enemySprite.localScale = new Vector3(-1f, 1f, 1f);
        else if (forceX <= -0.01f)
            enemySprite.localScale = new Vector3(1f, 1f, 1f);
    }

    void Jump(float xForce)
    {
        if (CheckJump())
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(xForce, jumpForce));
        }
    }
    private bool CheckJump()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpCheckDistance, layerMask);

        if (hit)
        {
            Debug.DrawRay(transform.position, Vector2.down, Color.blue, jumpCheckDistance);
            return true;
        } else
            return false;
    }
}
