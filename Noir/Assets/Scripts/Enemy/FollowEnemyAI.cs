using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

// This is a basic enemy AI that allows an enemy to follow a player and jump when needed
// Enemy will rotate (Flips X) towards target
public class FollowEnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;

    [Header("Stats")]
    [Tooltip("This enemys speed")]
    public float baseSpeed;
    public float currentSpeed;
    [Tooltip("How high this enemy will jump")]
    public float baseJumpForce;
    public float currentJumpForce;
    [Tooltip("How much higher this enemys target needs to be compared to this object (Y axis)")]
    public float targetJumpYThreshold;
    [Tooltip("Minimum total distance this enemys target needs be compared to this enemy to jump")]
    public float targetJumpDistanceThreshold;
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
        // Attempts to jump if this enemy can jump, the target is withing the jumpCheckDistance, and the target positions Y is a certain amount higher than this enemy (defined by targetJumpYThreshold)... When the player is in proximity of the enemy, Jump (With some extra parameteres) 
        if (target.position.y > this.rb.position.y + targetJumpYThreshold && canJump && Vector2.Distance(this.transform.position, target.transform.position) <= targetJumpDistanceThreshold)
        {
            Jump(forceX, false);
            Debug.Log("Jumping1");
        }
        // Tries to jump when the enemy is not moving and there is a wall infront of the enemy
        if (canJump && this.rb.velocity.normalized.magnitude == 0)
        {
            Jump(forceX, true);
            Debug.Log("Jumping2");
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (path.vectorPath[currentWaypoint].x >= transform.position.x)
            enemySprite.localScale = new Vector3(-1f, 1f, 1f);
        else if (path.vectorPath[currentWaypoint].x < transform.position.x)
            enemySprite.localScale = new Vector3(1f, 1f, 1f);
        else
            enemySprite.localScale = enemySprite.localScale;
    }

    // check front will check if there is a wall infront of an enemy (Normally when the player is not near the enemy to prevent the enemy from getting stuck on walls)
    void Jump(float xForce, bool checkFront)
    {
        if (CheckJump(checkFront))
            ApplyJump();

        void ApplyJump()
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(xForce, currentJumpForce));

            StartCoroutine(JumpCooldown());
        }
    }
    private bool CheckJump(bool forwardRay)
    {
        float drawRayExtraRange = 1;
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, jumpCheckDistance, layerMask);

        // Forward Rays
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, jumpCheckDistance, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, jumpCheckDistance, layerMask);

        if (hitDown && !forwardRay)
        {
            Debug.DrawRay(transform.position, Vector2.down, Color.blue, jumpCheckDistance + drawRayExtraRange);
            return true;
        } 
        else if (forwardRay && hitDown && hitLeft || forwardRay && hitDown && hitRight)
        {
            if (hitLeft)
                Debug.DrawRay(transform.position, Vector2.left, Color.blue, jumpCheckDistance + drawRayExtraRange);
            if (hitRight)
                Debug.DrawRay(transform.position, Vector2.right, Color.blue, jumpCheckDistance + drawRayExtraRange);

            Debug.DrawRay(transform.position, Vector2.down, Color.blue, jumpCheckDistance + drawRayExtraRange);
            return true;
        }
        else
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
            target.GetComponent<PlayerHealth>().TakeDamage(enemyScript.currentDamage);
        }
    }
}