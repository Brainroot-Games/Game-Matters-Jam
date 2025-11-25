using System.Collections.Generic;
using UnityEngine;
using static Emotions;

public class MinionSpawner : MonoBehaviour
{
    public int minCount = 1;
    public int maxCount = 5;
    public float spawnRadius = 8f;

    public GameObject minionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int minionCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < minionCount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition.z = -1;
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity, transform);
            minion.GetComponent<Minion>().Emotion = (Emotion)Random.Range(1, 6);
        }
    }
}
