using UnityEditor;
using UnityEngine;
using static Emotions;

public class MinionSpawner : MonoBehaviour
{
    public int minCount = 1;
    public int maxCount = 5;
    public float spawnRadius = 3f;
    public GameObject minionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int minionCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < minionCount; i++)
        {
            GameObject minion = Instantiate(minionPrefab, GetRandomSpawnPosition(), Quaternion.identity, transform);
            minion.GetComponent<Minion>().Emotion = GetRandomEmotion();
        }
    }

    private Vector2 GetRandomSpawnPosition() =>
        (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.position = GetRandomSpawnPosition();
        }
    }

    private void OnValidate()
    {
        maxCount = Mathf.Max(minCount, maxCount);
        minCount = Mathf.Clamp(minCount, 0, maxCount);
        spawnRadius = Mathf.Max(0, spawnRadius);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, spawnRadius);
    }
}
