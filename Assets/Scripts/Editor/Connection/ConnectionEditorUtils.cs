using UnityEditor;
using UnityEngine;
using static ConnectionList;

public static class ConnectionEditorUtils
{
    private static readonly float offset = 0.3f;
    private static readonly float roundFactor = Mathf.Pow(10f, 2);

    public static bool RegenerateConnections(ConnectionList connectionList)
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Regenerate Connections");
        int group = Undo.GetCurrentGroup();

        Transform connectionsParent = GameObject.FindWithTag("Connections").transform;
        ClearConnections(connectionsParent);

        bool allValid = GenerateConnections(connectionList);

        Undo.CollapseUndoOperations(group);

        return allValid;
    }

    public static void ClearConnections(Transform connectionsParent)
    {
        for (int i = connectionsParent.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(connectionsParent.GetChild(i).gameObject);
        }
    }

    private static bool GenerateConnections(ConnectionList connectionList)
    {
        Transform neuronsParent = GameObject.FindWithTag("Neurons").transform;

        bool allValid = true;

        foreach (ConnectionData connection in connectionList.connections)
        {
            bool valid = connection.IsValid(neuronsParent);

            if (valid)
            {
                Neuron fromNeuron = neuronsParent.GetChild(connection.fromNeuronId - 1).GetComponent<Neuron>();
                Neuron toNeuron = neuronsParent.GetChild(connection.toNeuronId - 1).GetComponent<Neuron>();
                if (fromNeuron != null && toNeuron != null)
                {
                    GenerateConnection(connectionList.connectionPrefab, fromNeuron, toNeuron);
                }
            }
            else
            {
                Debug.LogWarning($"Connection {connection.fromNeuronId}-{connection.toNeuronId}" +
                    $" is invalid and has not been generated.");
            }

                allValid &= valid;
        }

        return allValid;
    }

    public static void GenerateConnection(GameObject connectionPrefab, Neuron fromNeuron, Neuron toNeuron)
    {
        GameObject connectionObject = (GameObject)PrefabUtility.InstantiatePrefab(connectionPrefab);

        connectionObject.transform.SetParent(GameObject.FindWithTag("Connections").transform);

        ColliderDistance2D distance = Physics2D.Distance(
            fromNeuron.GetComponentInChildren<EdgeCollider2D>(),
            toNeuron.GetComponentInChildren<EdgeCollider2D>());

        Vector2 fromEdge = ApplyOffset(distance.pointA, distance.normal);
        Vector2 toEdge = ApplyOffset(distance.pointB, -distance.normal);

        connectionObject.transform.SetPositionAndRotation(
            GetRoundedMidpoint(fromEdge, toEdge),
            GetRoundedRightAlignment(distance.normal));

        Connection connection = connectionObject.GetComponent<Connection>();
        connection.scale = GetRoundedScale(fromEdge, toEdge, connection.LocalLength);

        SetNeurons(connection, fromNeuron, toNeuron);

        connection.Validate();

        Undo.RegisterCreatedObjectUndo(connectionObject, "Generate Connection");
    }

    private static Vector2 ApplyOffset(Vector2 point, Vector2 direction)
    {
        return point + direction * offset;
    }

    private static Vector2 GetRoundedMidpoint(Vector2 from, Vector2 to)
    {
        Vector2 midpoint = (from + to) * 0.5f;
        midpoint = new Vector2(
            Mathf.Round(midpoint.x * roundFactor) / roundFactor,
            Mathf.Round(midpoint.y * roundFactor) / roundFactor);

        return midpoint;
    }

    private static Quaternion GetRoundedRightAlignment(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, Mathf.Round(angle));
    }

    private static float GetRoundedScale(Vector2 from, Vector2 to, float connectionLocalLength)
    {
        float scale = (from - to).magnitude / connectionLocalLength;
        return Mathf.Round(scale * roundFactor) / roundFactor;
    }

    private static void SetNeurons(Connection connection, Neuron fromNeuron, Neuron toNeuron)
    {
        Neuron[] neurons = connection.neurons;
        neurons[0] = fromNeuron;
        neurons[1] = toNeuron;
    }
}
