using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

// This is a basic enemy AI that allows an enemy to follow a player and jump when needed (with animations)
// Enemy will rotate (Flips X) towards target
public class FollowEnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;
    public Animator animator;
    public AnimationClip spawningAnimation;

    [Header("Movement")]
    [Tooltip("This enemys speed")]
    public float baseSpeed;
    public float speed;
    public bool facingRight;
    public bool canTurn;

    [Header("Jumping")]
    [Tooltip("How high this enemy will jump")]
    public float baseJumpForce;
    public float currentJumpForce;
    [Tooltip("Used in the jump raycasting, how far the ray will be cast forward to check for ground")]
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

    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        animator = enemySprite.GetComponent<Animator>();
        enemyScript = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        StartCoroutine(FreezeEnemy(spawningAnimation.length));
        StartCoroutine(GetTarget());
        IEnumerator GetTarget()
        {
            yield return new WaitForSeconds(.01f);

            target = this.gameObject.GetComponent<Enemy>().target;
            InvokeRepeating(nameof(UpdatePath), 0f, .5f);
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void Update()
    {
        GravityMultiplier();
        ApplyDifficultyRating();

        // walking animation
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void ApplyDifficultyRating()
    {
        speed = Mathf.Clamp(baseSpeed * endlessModeScript.difficultyMultiplier, baseSpeed, baseSpeed * 3f);
        currentJumpForce = Mathf.Clamp(baseJumpForce * (endlessModeScript.difficultyMultiplier / 1.25f), baseJumpForce, baseJumpForce * 1.2f);
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

        if (enemyScript.canMove && !enemyScript.frozen)
        {
            // Move enemy along the X axis, not Y
            rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);
        }

        // Jumping
        // Tries to jump when the enemy is not moving and there is a wall infront of the enemy, raycast checks for wall
        if (enemyScript.canJump && this.rb.velocity.normalized.magnitude == 0 && !enemyScript.frozen &&
        (Physics2D.Raycast(transform.position, Vector3.right, jumpCheckDistance, layerMask) || Physics2D.Raycast(transform.position, Vector3.left, jumpCheckDistance, layerMask)))
        {
            Jump(forceX);
            Debug.DrawRay(transform.position, Vector3.right * jumpCheckDistance, Color.blue);
            Debug.DrawRay(transform.position, Vector3.left * jumpCheckDistance, Color.yellow);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (target.transform.position.x >= transform.position.x && canTurn)
        {
            enemySprite.localScale = new Vector3(-1f, 1f, 1f);
            facingRight = true;
        }
        else if (target.transform.position.x < transform.position.x && canTurn)
        {
            enemySprite.localScale = new Vector3(1f, 1f, 1f);
            facingRight = false;
        }
        else if (target.transform.position.x == transform.position.x)
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
        }
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    private void GravityMultiplier()
    {
        if (this.rb.velocity.normalized[1] < 0)
            rb.gravityScale = gravityFallMultiplier;
        else
            rb.gravityScale = 1;
    }

    // freezes the enemy for a set amount of time, usually for the length of an animation clip
    public IEnumerator FreezeEnemy(float time)
    {
        rb.velocity = Vector3.zero;
        enemyScript.frozen = true;

        yield return new WaitForSeconds(time);

        enemyScript.frozen = false;
    }
}