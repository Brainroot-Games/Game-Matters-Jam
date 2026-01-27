using UnityEditor;
using UnityEngine;

public class Neuron : MonoBehaviour
{
    private const string PREFIX = "Neuron";

    [Tooltip("Progressive Neuron ID" +
        " (automatically set on validate)")]
    [Min(0)]
    public int id = 0;

    [Header("Gizmos Settings")]
    [Min(0)]
    public int labelFontSize = 30;

    private MinionSpawner minionSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minionSpawner = GetComponentInChildren<MinionSpawner>(true);
        SetSpawnerActive(false);
    }

    public void SetSpawnerActive(bool active)
    {
        if (minionSpawner != null)
        {
            minionSpawner.gameObject.SetActive(active);
        }
    }

    private void OnValidate()
    {
        if (PrefabUtility.GetPrefabInstanceStatus(this) == PrefabInstanceStatus.Connected)
        {
            id = transform.GetSiblingIndex() + 1;
            name = PREFIX + " " + id;
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.background = Texture2D.whiteTexture;
        labelStyle.normal.textColor = Color.purple;
        labelStyle.fontSize = labelFontSize;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position, id.ToString(), labelStyle);
    }
}
