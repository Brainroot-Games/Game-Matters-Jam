using UnityEngine;

public class Synapse : MonoBehaviour
{
    [Min(0)]
    public float moveTime = 1f;
    public Transform teleport;
    public Transform spawn;

    [Tooltip("Local length on the x-axis")]
    public float localLength = 4f;

    public Neuron FromNeuron { get; set; }
    public Neuron ToNeuron { get; set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!player.IsInSynapse)
            {
                EventManager.Instance.onPlayerInSynapse.Invoke(this);
            }
        }
    }
}
