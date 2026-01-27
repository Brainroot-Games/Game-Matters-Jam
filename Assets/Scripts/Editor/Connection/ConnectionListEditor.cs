using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConnectionList))]
public class ConnectionListEditor : Editor
{
    private const string OPERATION_NAME = "Regenerate Connections";

    private bool warning = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button(OPERATION_NAME))
        {
            ConnectionList connectionList = (ConnectionList)target;
            warning = !ConnectionEditorUtils.RegenerateConnections(connectionList);
        }

        if (warning)
        {
            EditorGUILayout.HelpBox(
                    $"All Neuron IDs in each Connection must be valid and different.",
                    MessageType.Warning);
        }
    }
}
