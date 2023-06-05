using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public int currentHealth;
    public int maxHealth;
    private float attackTime;
    public float timeBetweenAttack;
    private bool canMove;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform checkEnemy;
    public LayerMask whatIsEnemy;
    public float range;

    public static PlayerController instance;

    private void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY);
        if (movement != Vector2.zero)
        {
            anim.SetFloat("lastInputX", movement.x);
            anim.SetFloat("lastInputY", movement.y);
        }
        if (Input.GetAxis("Horizontal") > 0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
        }
        else if (Input.GetAxis("Horizontal") < -0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
        }
        if (Input.GetAxis("Vertical") > 0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
        }
        else if (Input.GetAxis("Vertical") < -0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
        }

        rb.MovePosition(rb.position + movement.normalized * speed * Time.deltaTime);
    }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= attackTime)
            {
                rb.velocity = Vector2.zero;
                anim.SetTrigger("attack");

                StartCoroutine(Delay());

                IEnumerator Delay()
                {
                    canMove = false;
                    yield return new WaitForSeconds(.5f);
                    canMove = true;
                }

                attackTime = Time.time + timeBetweenAttack;
            }
        }

    }

    public void OnAttack()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(checkEnemy.position, 0.5f, whatIsEnemy);

        foreach (Collider2D enemy in enemies)
        {
            // Apply damage or other effects to the enemy
        }
    }

}
