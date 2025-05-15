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
    [Tooltip("Does this enemy deal damage when it comes in contact with the player")]
    public bool contactDamage;

    [Header("Movement")]
    [Tooltip("This enemys speed")]
    public float baseSpeed;
    public float speed;

    [Header("Jumping")]
    [Tooltip("How high this enemy will jump")]
    public float baseJumpForce;
    public float currentJumpForce;
    [Tooltip("The maximum time between jumps")]
    public float jumpCooldown;
    [Tooltip("Used in the jump raycasting, how far the ray will be cast downwards to check for ground")]
    public float jumpCheckDistance;
    [Tooltip("Gravity increase when this enemy is falling")]
    public float gravityFallMultiplier;
    [Tooltip("What object layer is getting checked for raycast collisions")]
    public LayerMask layerMask;

    [Header("Pathfinding")]
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
        enemyScript.canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        enemyScript.canJump = true;
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
            InvokeRepeating(nameof(UpdatePath), 0f, .5f);
        }

        enemyScript.canJump = true;
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
        GravityMultiplier();
        ApplyDifficultyRating();
    }

    private void ApplyDifficultyRating()
    {
        speed = Mathf.Clamp(baseSpeed * endlessModeScript.difficultyMultiplier, baseSpeed, baseSpeed * 3f);
        currentJumpForce =  Mathf.Clamp(baseJumpForce * (endlessModeScript.difficultyMultiplier / 1.25f), baseJumpForce, baseJumpForce * 1.2f);
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

        if (enemyScript.canMove)
        {
            // Move enemy along the X axis, not Y
            rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);
        }

        // Jumping
        // Tries to jump when the enemy is not moving and there is a wall infront of the enemy
        if (enemyScript.canJump && this.rb.velocity.normalized.magnitude == 0)
        {
            Jump(forceX);
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
    void Jump(float xForce)
    {
        if (enemyScript.canJump)
        {
            ApplyJump();
        }

        void ApplyJump()
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(xForce, currentJumpForce));

            StartCoroutine(JumpCooldown());
        }
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && contactDamage)
        {
            Debug.Log("Dealing damage to player");
            target.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

    private void GravityMultiplier()
    {
        if (this.rb.velocity.normalized[1] < 0)
            rb.gravityScale = gravityFallMultiplier;
        else
            rb.gravityScale = 1;
    }
}