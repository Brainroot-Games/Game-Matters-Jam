using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private const string PREFIX = "Connection";

    public const int SYNAPSES_SIZE = 2;
    public const int NEURONS_SIZE = SYNAPSES_SIZE;

    [Tooltip("Local scale on the x-axis" +
        " (set it here to automatically set the spawns positions)")]
    [Min(0.01f)]
    public float scale = 1f;

    [Header("Gizmos Settings")]
    [Min(0)]
    public float lineThickness = 10f;

    [Header("Connection Settings")]
    [Tooltip("Synapses (fixed size of 2)")]
    public Synapse[] synapses = new Synapse[SYNAPSES_SIZE];

    [Tooltip("Connected Neurons (fixed size of 2)" +
        " (automatically set with button or tools)")]
    public Neuron[] neurons = new Neuron[NEURONS_SIZE];

    public float LocalLength => synapses.Select(synapse => synapse.localLength).Sum();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetSynapsesNeurons();
    }

    public void Validate()
    {
        Scale();
        SetArraySizes();
        SetSpawnPositions();

        if (PrefabUtility.GetPrefabInstanceStatus(this) == PrefabInstanceStatus.Connected)
        {
            SetName();
        }
    }

    private void SetSynapsesNeurons()
    {
        for (int i = 0; i < synapses.Length; i++)
        {
            synapses[i].FromNeuron = neurons[i];
            synapses[i].ToNeuron = neurons[neurons.Length - 1 - i];
        }
    }

    private void Scale()
    {
        scale = Mathf.Max(0.01f, scale);
        transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
    }

    private void SetArraySizes()
    {
        SetArraySize(ref synapses, SYNAPSES_SIZE);
        SetArraySize(ref neurons, NEURONS_SIZE);
    }

    private void SetArraySize<T>(ref T[] array, int size)
    {
        if (array.Length != size)
        {
            T[] newArray = new T[size];
            Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
            array = newArray;
        }
    }

    private void SetSpawnPositions()
    {
        foreach (Synapse synapse in synapses)
        {
            if (synapse != null && scale != 0)
            {
                synapse.spawn.GetComponent<Spawn>().SetPosition(scale);
            }
        }
    }

    private void SetName()
    {
        name = PREFIX + " ";

        foreach (Neuron neuron in neurons)
        {
            if (neuron != null)
            {
                name += neuron.id + "-";
            }
        }

        name = name[..^1];
    }

    private void OnValidate()
    {
        Validate();
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.purple;
        if (neurons[0] != null && neurons[1] != null)
        {
            Handles.DrawLine(neurons[0].transform.position, neurons[1].transform.position, lineThickness);
        }
    }
}
