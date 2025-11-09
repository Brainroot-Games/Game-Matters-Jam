using UnityEngine;
using static Emotions;

public class EnemySpawner : MonoBehaviour
{
    public int minCount = 1;
    public int maxCount = 5;
    public GameObject enemyPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int enemyCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * Utils.boundsRay;
            spawnPosition.z = -1;
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            enemy.GetComponent<Enemy>().Emotion = (Emotion)Random.Range(1, 6);
        }
    }
}
