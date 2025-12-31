using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float radius = 0.5f;

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
    }
}
