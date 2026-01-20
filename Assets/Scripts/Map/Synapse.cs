using UnityEngine;

public class Synapse : MonoBehaviour
{
    public float moveTime = 1f;
    public SpriteRenderer spriteRenderer;
    public Transform teleport;
    public Transform spawn;

    [Header("Local length on the x-axis" +
        "\n(automatically set on validate)")]
    public float localLength = 4f;

    [Header("Destination Neuron" +
        "\n(automatically set with Connection button" +
        "\n\"" + Connection.BUTTON_TEXT + "\")")]
    public Neuron toNeuron;

    private CameraManager cameraManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!player.IsInSynapse)
            {
                player.MoveInSynapse(teleport, spawn, moveTime);
                cameraManager.MoveToTarget(toNeuron.transform.position, moveTime);
                MapManager.Instance.ChangeNeuron(toNeuron);
            }
        }
    }

    private void OnValidate()
    {
        moveTime = Mathf.Max(0, moveTime);

        if (spriteRenderer != null)
        {
            localLength = Mathf.Round(spriteRenderer.sprite.bounds.size.x);
        }
    }
}
