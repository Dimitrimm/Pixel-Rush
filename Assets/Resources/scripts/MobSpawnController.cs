using UnityEngine;

public class MobSpawnController : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public int numberOfPrefabs = 5;
    public float spawnInterval = 5f;
    public float yPosition = 5f;
    public float xMinRange = -10f;
    public float xMaxRange = 10f;
    public Transform playerTransform;
    public float minDistanceFromPlayer = 10f;

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnPrefabs();
            timer = spawnInterval;
        }
    }

    void SpawnPrefabs()
    {
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float randomX;

            do
            {
                randomX = Random.Range(xMinRange, xMaxRange);
            }
            while (Mathf.Abs(randomX - playerTransform.position.x) < minDistanceFromPlayer);

            Vector2 spawnPosition = new Vector2(randomX, yPosition);
            GameObject prefabToSpawn = Random.value > 0.5f ? prefab1 : prefab2;

            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
