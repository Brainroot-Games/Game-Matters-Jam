using UnityEngine;
using static Emotions;

public class MinionSpawner : MonoBehaviour
{
    public int minCount = 1;
    public int maxCount = 5;
    public float spawnRadius = 4f;

    public GameObject minionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int minionCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < minionCount; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity, transform);
            minion.GetComponent<Minion>().Emotion = (Emotion)Random.Range(1, 6);
        }
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, spawnRadius);
    }
}
