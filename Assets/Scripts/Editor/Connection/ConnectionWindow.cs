using UnityEditor;
using UnityEngine;

public class ConnectionWindow : EditorWindow
{
    private const string TOOL_NAME = "Connection Manager";
    private const int TAB_COUNT = 3;

    private static readonly string[] tabNames = new string[TAB_COUNT]
    {
        "Single",
        "Multiple",
        "Clear"
    };

    private static readonly string[] labels = new string[TAB_COUNT]
    {
        "Single Connection Generation",
        "Multiple Connections Regeneration",
        "Clear Connections"
    };

    [SerializeField]
    private GameObject connectionPrefab;
    [SerializeField]
    private ConnectionList connectionList;

    private Neuron fromNeuron;
    private Neuron toNeuron;

    private GameObject connectionsParent;

    private int tab = 0;

    private readonly bool[] foldouts = new bool[TAB_COUNT]
    {
        true,
        true,
        true
    };

    private bool warning = false;

    [MenuItem("Tools/Connections/" + TOOL_NAME)]
    public static void OpenWindow()
    {
        GetWindow<ConnectionWindow>(TOOL_NAME);
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        tab = GUILayout.Toolbar(tab, tabNames);

        GUILayout.Space(5);

        GUILayout.Label(labels[tab], EditorStyles.boldLabel);

        GUILayout.Space(5);

        foldouts[tab] = EditorGUILayout.Foldout(foldouts[tab], "Settings");

        switch (tab)
        {
            case 0:
                OnSingleTab();
                break;
            case 1:
                OnMultipleTab();
                break;
            case 2:
                OnClearTab();
                break;
        }
    }

    private void OnSingleTab()
    {
        bool enabled = false;

        if (foldouts[tab])
        {
            enabled = connectionPrefab == null ||
                connectionPrefab.GetComponent<Connection>() == null;

            using (new EditorGUI.DisabledScope(!enabled))
            {
                connectionPrefab = (GameObject)EditorGUILayout.ObjectField(
                    "Connection Prefab",
                    connectionPrefab,
                    typeof(GameObject),
                    false);
            }

            if (connectionPrefab != null && connectionPrefab.GetComponent<Connection>() == null)
            {
                EditorGUILayout.HelpBox(
                    "Connection Prefab must have a Connection component attached.",
                    MessageType.Error);
            }

            fromNeuron = (Neuron)EditorGUILayout.ObjectField(
                "From Neuron",
                fromNeuron,
                typeof(Neuron),
                true);

            toNeuron = (Neuron)EditorGUILayout.ObjectField(
                "To Neuron",
                toNeuron,
                typeof(Neuron),
                true);

            if (fromNeuron != null &&
                toNeuron != null &&
                fromNeuron == toNeuron)
            {
                EditorGUILayout.HelpBox(
                    "From Neuron and To Neuron must be different.",
                    MessageType.Error);
            }
        }

        GUILayout.Space(5);

        enabled =
            connectionPrefab != null &&
            connectionPrefab.GetComponent<Connection>() != null &&
            fromNeuron != null &&
            toNeuron != null &&
            fromNeuron != toNeuron;

        GUIContent content = new GUIContent(
            "Generate Connection",
            enabled ? "" : "Assign all required fields");

        using (new EditorGUI.DisabledScope(!enabled))
        {
            if (GUILayout.Button(content))
            {
                ConnectionEditorUtils.GenerateConnection(connectionPrefab, fromNeuron, toNeuron);
            }
        }
    }

    private void OnMultipleTab()
    {
        if (foldouts[tab])
        {
            connectionList = (ConnectionList)EditorGUILayout.ObjectField(
                "Connection List",
                connectionList,
                typeof(ConnectionList),
                false);
        }

        GUILayout.Space(5);

        bool enabled = connectionList != null;

        GUIContent content = new GUIContent(
            "Regenerate Connections",
            enabled ? "" : "Assign all required fields");

        using (new EditorGUI.DisabledScope(!enabled))
        {
            if (GUILayout.Button(content))
            {
                warning = !ConnectionEditorUtils.RegenerateConnections(connectionList);
            }
        }

        if (warning)
        {
            EditorGUILayout.HelpBox(
                    $"All Neuron IDs in each Connection in Connection List must be valid and different.",
                    MessageType.Warning);
        }
    }

    private void OnClearTab()
    {
        connectionsParent = GameObject.FindWithTag("Connections");

        bool enabled =
            connectionsParent == null ||
            !connectionsParent.CompareTag("Connections");

        if (foldouts[tab])
        {
            using (new EditorGUI.DisabledScope(!enabled))
            {
                connectionsParent = (GameObject)EditorGUILayout.ObjectField(
                    "Connections Parent",
                    connectionsParent,
                    typeof(GameObject),
                    true);
            }

            if (connectionsParent != null && !connectionsParent.CompareTag("Connections"))
            {
                EditorGUILayout.HelpBox(
                    "Connections Parent must have the \"Connections\" tag assigned.",
                    MessageType.Error);
            }
        }

        GUILayout.Space(5);

        enabled = !enabled;

        GUIContent content = new GUIContent(
            "Clear Connections",
            enabled ? "" : "Assign all required fields");

        using (new EditorGUI.DisabledScope(!enabled))
        {
            if (GUILayout.Button(content))
            {
                ConnectionEditorUtils.ClearConnections(connectionsParent.transform);
            }
        }
    }
}
