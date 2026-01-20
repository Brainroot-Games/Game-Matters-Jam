using System;
using UnityEditor;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private const string PREFIX = "Connection";

    public const string BUTTON_TEXT = "Connect Neurons";
    public const int SYNAPSES_SIZE = 2;
    public const int NEURONS_SIZE = SYNAPSES_SIZE;

    [Header("Local scale on the x-axis" +
        "\n(set it here to automatically set the spawns positions)")]
    public float length = 1.0f;

    [Header("Gizmos Settings")]
    public float lineThickness = 10f;

    [Header("Synapses (fixed size of 2)")]
    public Synapse[] synapses = new Synapse[SYNAPSES_SIZE];

    [Header("Connected Neurons (fixed size of 2)" +
        "\n(automatically set with button \"" + BUTTON_TEXT + "\")")]
    public Neuron[] neurons = new Neuron[NEURONS_SIZE];

    private void Scale()
    {
        length = Mathf.Max(0, length);
        transform.localScale = new Vector3(length, transform.localScale.y, transform.localScale.z);
    }

    private void SetArraysSize()
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

    private void SetSpawnPosition()
    {
        foreach (Synapse synapse in synapses)
        {
            if (synapse != null && length != 0)
            {
                Transform spawnTransform = synapse.spawn;
                Spawn spawn = spawnTransform.GetComponent<Spawn>();
                spawnTransform.localPosition = new Vector3(-(synapse.localLength + spawn.offset / length),
                    spawnTransform.localPosition.y,
                    spawnTransform.localPosition.z);
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
        Scale();
        SetArraysSize();
        SetSpawnPosition();

        if (PrefabUtility.GetPrefabInstanceStatus(this) == PrefabInstanceStatus.Connected)
        {
            SetName();
        }
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
