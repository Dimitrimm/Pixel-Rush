using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;
    public float knockbackForce = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, knockbackDirection);

                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    player.IncrementKillCount();
                }

                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
