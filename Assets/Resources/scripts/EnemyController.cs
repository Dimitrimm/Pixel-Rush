using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public int health = 3;
    public float knockbackForce = 5f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public bool canShoot = false;

    private Transform player;
    private Rigidbody2D rb;
    private bool isChasing = false;
    private Animator animator;
    private float shootInterval = 1.5f;
    private float shootTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        isChasing = distanceToPlayer <= detectionRange;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (canShoot && isChasing)
        {
            HandleShooting();
        }
        else if (isChasing)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        transform.localScale = new Vector3(rb.velocity.x > 0 ? 1 : -1, 1, 1);
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Vector2 shootDirection = (player.position - transform.position).normalized;

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = shootDirection * 10f;
    }

    public void TakeDamage(int damage, Vector2 knockbackDir)
    {
        health -= damage;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            knockbackDirection = new Vector2(knockbackDirection.x, 0.1f);

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(1, knockbackDirection);
            }
        }
    }
}
