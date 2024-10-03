using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.1f;

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public int maxBullets = 10;
    private int currentBullets;

    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI killCountText;

    public int health = 5;
    public float knockbackForce = 5f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.2f;

    private int killCount = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentBullets = maxBullets;
        UpdateBulletCountUI();
        UpdateKillCountUI();
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.anyKeyDown) RestartGame();
            return;
        }

        if (!isKnockedBack)
        {
            HandleMovement();
            HandleJump();
            HandleShooting();
            if (health <= 0) Die();
        }
        else
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0) isKnockedBack = false;
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        if (moveInput != 0) transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isJumping = !isGrounded;
        animator.SetBool("IsJumping", isJumping);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.velocity = Vector2.up * jumpForce;
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.P) && currentBullets > 0)
        {
            Shoot();
            currentBullets--;
            UpdateBulletCountUI();
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (transform.localScale.x < 0)
        {
            projectile.transform.localScale = new Vector3(-1, 1, 1);
            projectile.GetComponent<ProjectileController>().speed *= -1;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDir)
    {
        health -= damage;

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(knockbackDir.x * 25f, knockbackDir.y * 0.2f);
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateBulletCountUI()
    {
        bulletCountText.text = "Bullets: " + currentBullets;
    }

    void UpdateKillCountUI()
    {
        killCountText.text = "Kills: " + killCount;
    }

    public void IncrementKillCount()
    {
        killCount++;
        UpdateKillCountUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentBullets = Mathf.Min(currentBullets + 4, maxBullets);
            UpdateBulletCountUI();
            IncrementKillCount();
        }
    }
}
