using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

public class Enemy : MonoBehaviour
{
    public float speed;
    private float playerDetectTime;
    public float playerDetectRate;
    public float chaseRange;
    public float attackRange;
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private PlayerController targetPlayer;
    public float nextWaypointDistance = 2f;
    private Pathfinding.Path path;
    private int currentWaypoint = 0;
    private bool reachEndPath = false;
    private Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && targetPlayer != null)
        {
            seeker.StartPath(rb.position, targetPlayer.transform.position, OnPathComplete);
        }
    }

    private void FixedUpdate()
    {
        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                // Attack
                rb.velocity = Vector2.zero;
            }
            else if (dist > attackRange)
            {
                if (path == null)
                    return;
                if (currentWaypoint < path.vectorPath.Count)
                {
                    reachEndPath = false;

                    Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                    Vector2 force = direction * speed * Time.fixedDeltaTime;

                    rb.velocity = force;

                    float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                    if (distance < nextWaypointDistance)
                    {
                        currentWaypoint++;
                    }
                }
                else
                {
                    reachEndPath = true;
                    rb.velocity = Vector2.zero;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }

        DetectPlayer();
    }
    void DetectPlayer()
    {
        if (Time.time - playerDetectTime > playerDetectRate)
        {
            playerDetectTime = Time.time;
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                if (player != null)
                {
                    float dist = Vector2.Distance(transform.position, player.transform.position);
                    if (player == targetPlayer)
                    {
                        if (dist > chaseRange)
                        {
                            targetPlayer = null;
                        }
                    }
                    else if (dist < chaseRange)
                    {
                        if (targetPlayer == null)
                            targetPlayer = player;
                    }
                }
            }
        }
    }
}