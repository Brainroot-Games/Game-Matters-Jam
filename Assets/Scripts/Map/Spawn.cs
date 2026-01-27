using UnityEditor;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float offset = 0.6f;
    public Synapse synapse;

    public void SetPosition(float connectionScale)
    {
        transform.localPosition = new Vector3(-(synapse.localLength + offset / connectionScale),
                transform.localPosition.y,
                transform.localPosition.z);
    }

    private void OnValidate()
    {
        if (synapse != null)
        {
            Connection connection = GetComponentInParent<Connection>();
            float connectionScale = (connection != null) ? connection.scale : 1f;
            SetPosition(connectionScale);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(transform.position, Vector3.back, offset);
    }
}
