using UnityEngine;

public class Synapse : MonoBehaviour
{
    private Vector2 spawnPoint;
    private CameraManager cameraManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = transform.GetChild(1).position;
        cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.transform.position = spawnPoint;
            cameraManager.Target = spawnPoint;
        }
    }
}
