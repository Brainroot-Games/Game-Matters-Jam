using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 target;
    private Vector3 offset;
    private float speed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = transform.position;
        offset = transform.position;
    }

    private void LateUpdate()
    {
        if (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target,
                speed * Time.deltaTime);
        }
    }

    public void OnPlayerInSynapse(Synapse synapse)
    {
        target = synapse.ToNeuron.transform.position + offset;
        speed = Utils.GetSpeed(transform.position, target, synapse.moveTime);
    }
}
