using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FollowEnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;

    [Header("Stats")]
    [Tooltip("This enemys speed")]
    public float baseSpeed;
    public float currentSpeed;
    [Tooltip("How high/hard this enemy will jump")]
    public float baseJumpForce;
    public float currentJumpForce;
    [Tooltip("How much higher this enemys target needs to be compared to this object (Y axis)")]
    public float jumpThreshold;
    [Tooltip("The maximum time between jumps")]
    public float jumpCooldown;
    [Tooltip("Used in the jump raycasting, how far the ray will be cast downwards to check for ground")]
    public float jumpCheckDistance;
    // Can this enemy jump?
    public bool canJump;
    [Tooltip("What object layer is getting checked for raycast collisions")]
    public LayerMask layerMask;

    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    // OTHER: Referenced in Start()
    Seeker seeker;
    Rigidbody2D rb;
    Enemy enemyScript;
    EndlessMode endlessModeScript;

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

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
            InvokeRepeating("UpdatePath", 0f, .5f);
        }

        canJump = true;
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
    private void Update()
    {
        ApplyDifficultyRating();
    }

    private void ApplyDifficultyRating()
    {
        currentSpeed = baseSpeed * endlessModeScript.difficultyMultiplier;
        currentJumpForce =  Mathf.Clamp(baseJumpForce * (endlessModeScript.difficultyMultiplier / 1.25f), baseJumpForce, Mathf.Infinity);
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
        float forceX = direction.x * currentSpeed * Time.deltaTime;

        // Move enemy along the X axis, not Y
        rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);

        // Jumping
        if (target.position.y > rb.position.y + jumpThreshold && canJump)
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
            rb.AddForce(new Vector2(xForce, currentJumpForce));

            StartCoroutine(JumpCooldown());
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

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions"))
        {
            Debug.Log("Dealing damage to player");
            target.GetComponent<Player>().TakeDamage(enemyScript.currentDamage);
        }
    }
}
