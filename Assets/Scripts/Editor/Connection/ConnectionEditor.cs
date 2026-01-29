using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Connection))]
public class ConnectionEditor : Editor
{
    private const string OPERATION_NAME = "Connect Neurons";

    private readonly SerializedProperty[] synapsesProp = new SerializedProperty[Connection.SYNAPSES_SIZE];
    private readonly SerializedProperty[] neuronsProp = new SerializedProperty[Connection.NEURONS_SIZE];

    private void OnEnable()
    {
        for (int i = 0; i < synapsesProp.Length; i++)
        {
            synapsesProp[i] = serializedObject.FindProperty("synapses").GetArrayElementAtIndex(i);
        }

        for (int i = 0; i < neuronsProp.Length; i++)
        {
            neuronsProp[i] = serializedObject.FindProperty("neurons").GetArrayElementAtIndex(i);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button(OPERATION_NAME) &&
            PrefabUtility.GetPrefabInstanceStatus(serializedObject.targetObject) == PrefabInstanceStatus.Connected)
        {
            SetNeurons();
        }

        if (neuronsProp[0].objectReferenceValue == null ||
            neuronsProp[1].objectReferenceValue == null ||
            neuronsProp[0].objectReferenceValue == neuronsProp[1].objectReferenceValue)
        {
            EditorGUILayout.HelpBox(
                    $"Connected Neurons must be non-null and different.",
                    MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SetNeurons()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName(OPERATION_NAME);
        int group = Undo.GetCurrentGroup();

        for (int i = 0; i < synapsesProp.Length; i++)
        {
            Synapse synapse = (Synapse)synapsesProp[i].objectReferenceValue;
            if (synapse != null)
            {
                SetNeuronRaycast(neuronsProp[i], synapse.teleport.position);
                SetNeuronRaycast(neuronsProp[neuronsProp.Length - 1 - i], synapse.spawn.position);
            }
        }

        Undo.CollapseUndoOperations(group);
    }

    private void SetNeuronRaycast(SerializedProperty neuronProp, Vector2 origin)
    {
        Undo.RecordObject(serializedObject.targetObject, "Connect Neuron");
        neuronProp.objectReferenceValue = NeuronRaycast(origin);
    }

    private Neuron NeuronRaycast(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0, LayerMask.GetMask("Platform"));
        return hit ? hit.collider.GetComponentInParent<Neuron>() : null;
    }
}
