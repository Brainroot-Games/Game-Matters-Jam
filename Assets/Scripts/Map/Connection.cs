using UnityEngine;

public class Connection : MonoBehaviour
{
    public float scale = 1.0f;
    public float spawnOffset = 0.5f;
    public Transform[] spawnPoints = new Transform[2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn != null && scale != 0)
            {
                spawn.localPosition = new Vector2(spawn.localPosition.x, 0.5f + spawnOffset / scale);
            }
        }
    }
}
