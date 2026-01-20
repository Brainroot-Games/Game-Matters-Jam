using UnityEditor;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float offset = 0.6f;
    public Synapse synapse;

    private void OnValidate()
    {
        if (synapse != null)
        {
            transform.localPosition = new Vector3(-(synapse.localLength + offset),
                transform.localPosition.y,
                transform.localPosition.z);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(transform.position, Vector3.back, offset);
    }
}
