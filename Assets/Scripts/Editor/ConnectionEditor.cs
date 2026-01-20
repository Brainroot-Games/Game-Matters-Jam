using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Connection))]
public class ConnectionEditor : Editor
{
    private SerializedProperty[] synapsesProp = new SerializedProperty[Connection.SYNAPSES_SIZE];
    private SerializedProperty[] neuronsProp = new SerializedProperty[Connection.NEURONS_SIZE];

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

        EditorGUILayout.Space();

        if (GUILayout.Button(Connection.BUTTON_TEXT) &&
            PrefabUtility.GetPrefabInstanceStatus(serializedObject.targetObject) == PrefabInstanceStatus.Connected)
        {
            SetNeurons();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SetNeurons()
    {
        for (int i = 0; i < synapsesProp.Length; i++)
        {
            Synapse synapse = (Synapse)synapsesProp[i].objectReferenceValue;
            if (synapse != null)
            {
                SerializedObject serializedSynapse = new SerializedObject(synapse);
                serializedSynapse.Update();
                SerializedProperty toNeuronProp = serializedSynapse.FindProperty("toNeuron");

                RaycastHit2D hit = Physics2D.Raycast(synapse.spawn.position, Vector2.down, 0, LayerMask.GetMask("Platform"));
                if (hit)
                {
                    neuronsProp[neuronsProp.Length - 1 - i].objectReferenceValue =
                        hit.collider.GetComponentInParent<Neuron>();

                    toNeuronProp.objectReferenceValue =
                        neuronsProp[synapsesProp.Length - 1 - i].objectReferenceValue;
                }
                else
                {
                    neuronsProp[neuronsProp.Length - 1 - i].objectReferenceValue = null;
                    toNeuronProp.objectReferenceValue = null;
                }

                serializedSynapse.ApplyModifiedProperties();
            }
        }
    }
}
