using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// </summary>
public class MapManager : MonoBehaviour {
    public static MapManager _instance;

    [SerializeField] private Tilemap tileMap;
    private const int NumberOfSubDungeons = 20;
    private const int ClusterNumber = 5;
    
    private Dictionary<int, List<int>> clusterMapping;
    private int currentClusterID = -1;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        InitializeClusterMapping();
    }

    private void InitializeClusterMapping() {
        clusterMapping = new Dictionary<int, List<int>>();
        
        // Initialize each cluster with an empty list of sub-dungeon IDs
        for (int i = 0; i < ClusterNumber; i++) {
            clusterMapping[i] = new List<int>();
        }
        
        // Distribute sub-dungeons across clusters
        for (int subDungeonId = 0; subDungeonId < NumberOfSubDungeons; subDungeonId++) {
            int clusterId = subDungeonId % ClusterNumber;
            clusterMapping[clusterId].Add(subDungeonId);
        }
    }

    /// <summary>
    /// Called when player enters a new cluster
    /// </summary>
    public void OnEnterCluster(int clusterID) {
        if (clusterID < 0 || clusterID >= ClusterNumber) {
            Debug.LogError($"MapManager: Invalid cluster ID {clusterID}");
            return;
        }

        if (currentClusterID != clusterID) {
            currentClusterID = clusterID;
            
            // Play the corresponding cluster audio
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayClusterAudio(clusterID, useCrossfade: true);
            }
            
            Debug.Log($"MapManager: Entered cluster {clusterID}");
        }
    }

    /// <summary>
    /// Get the list of sub-dungeons in a specific cluster
    /// </summary>
    public List<int> GetSubDungeonsInCluster(int clusterID) {
        if (clusterMapping.ContainsKey(clusterID)) {
            return clusterMapping[clusterID];
        }
        return new List<int>();
    }

    /// <summary>
    /// Get the cluster ID that a specific sub-dungeon belongs to
    /// </summary>
    public int GetClusterForSubDungeon(int subDungeonID) {
        foreach (var kvp in clusterMapping) {
            if (kvp.Value.Contains(subDungeonID)) {
                return kvp.Key;
            }
        }
        return -1;
    }
}
