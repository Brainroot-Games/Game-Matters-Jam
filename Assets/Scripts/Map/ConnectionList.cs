using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConnectionList", menuName = "Scriptable Objects/Connection List")]
public class ConnectionList : ScriptableObject
{
    [Serializable]
    public struct ConnectionData
    {
        [Min(0)]
        public int fromNeuronId;
        [Min(0)]
        public int toNeuronId;

        public readonly bool IsValid(Transform neuronsParent) =>
            fromNeuronId > 0 && fromNeuronId <= neuronsParent.childCount &&
            toNeuronId > 0 && toNeuronId <= neuronsParent.childCount &&
            fromNeuronId != toNeuronId;
    }

    public GameObject connectionPrefab;

    public List<ConnectionData> connections = new List<ConnectionData>();
}
