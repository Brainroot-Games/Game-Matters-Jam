using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// with predefined node connections and door configurations
/// </summary>
public class MapManager : MonoBehaviour {
    public static MapManager Instance { get; private set; }

    #region Serialized Fields    
    [Header("Individual Node Sprites (21 total)")]
    [Tooltip("Assign a unique sprite for each node (Nodes 1-21)")]
    [SerializeField] private Sprite[] nodeSprites = new Sprite[21];

    [Header("Boss Node Sprite")]
    [Tooltip("Special sprite for Node 21 (Boss Node)")]
    [SerializeField] private Sprite bossNodeSprite;

    [Header("Sprite Renderer")]
    [Tooltip("The SpriteRenderer that will display the active room background")]
    [SerializeField] private SpriteRenderer roomSpriteRenderer;

    [Header("Room Image Resources Path")]
    [Tooltip("Path to node sprites in Resources folder (fallback if not assigned manually)")]
    [SerializeField] private string roomImageResourcePath = "Rooms/Nodes";

    [Header("Sprite Transition Settings")]
    [SerializeField] private bool enableSpriteTransition = true;
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    #endregion

    #region Constants
    private const int TotalNodes = 21;
    private const int ClusterCount = 5;
    private const int BossNodeID = 21;
    #endregion

    #region Private Fields
    // Node graph structure: nodeID -> list of connected nodeIDs
    private Dictionary<int, List<int>> nodeConnections;

    // Node metadata: nodeID -> door count
    private Dictionary<int, int> nodeDoorCounts;

    // Node to room sprite mapping: nodeID -> Sprite
    private Dictionary<int, Sprite> nodeRoomSprites;

    // Cluster assignments: clusterID -> list of nodeIDs
    private Dictionary<int, List<int>> clusterMapping;

    // Node to cluster lookup: nodeID -> clusterID
    private Dictionary<int, int> nodeToCluster;

    private int currentNodeID = -1;
    private int previousNodeID = -1;
    private int currentClusterID = -1;

    // Transition coroutine tracking
    private Coroutine activeTransition;
    #endregion

    #region Unity Lifecycle
    private void Awake() {
        // Singleton setup
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // Validate SpriteRenderer
        if (roomSpriteRenderer == null) {
            Debug.LogError("MapManager: RoomSpriteRenderer is not assigned! Please assign a SpriteRenderer in the Inspector.");
        }
    }

    private void Start() {
        InitializeMapConfiguration();
        LoadNodeSprites();
        AssignNodesToCluster();

        if (showDebugInfo) {
            LogMapConfiguration();
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialize the predefined 21-node graph structure
    /// </summary>
    private void InitializeMapConfiguration() {
        nodeConnections = new Dictionary<int, List<int>>
        {
            { 1,  new List<int> { 2, 13, 14 } },
            { 2,  new List<int> { 1, 3 } },
            { 3,  new List<int> { 2, 14, 15 } },
            { 4,  new List<int> { 15, 16 } },
            { 5,  new List<int> { 6, 16 } },
            { 6,  new List<int> { 5, 17 } },
            { 7,  new List<int> { 8, 17 } },
            { 8,  new List<int> { 7, 9, 17, 18 } },
            { 9,  new List<int> { 8, 18 } },
            { 10, new List<int> { 11, 19 } },
            { 11, new List<int> { 10, 19 } },
            { 12, new List<int> { 19, 20 } },
            { 13, new List<int> { 1, 20 } },
            { 14, new List<int> { 1, 3, 20 } },
            { 15, new List<int> { 3, 4, 21 } },
            { 16, new List<int> { 4, 5, 21 } },
            { 17, new List<int> { 6, 7, 8, 21 } },
            { 18, new List<int> { 8, 9, 10, 19 } },
            { 19, new List<int> { 10, 11, 12, 18, 21 } },
            { 20, new List<int> { 12, 13, 14, 21 } },
            { 21, new List<int> { 15, 16, 17, 19, 20 } }
        };

        // Calculate door counts based on connections
        nodeDoorCounts = new Dictionary<int, int>();
        foreach (var kvp in nodeConnections) {
            nodeDoorCounts[kvp.Key] = kvp.Value.Count;
        }

        Debug.Log($"MapManager: Initialized {TotalNodes} nodes with predefined connections");
    }

    /// <summary>
    /// Load individual node sprites from Inspector or Resources
    /// </summary>
    private void LoadNodeSprites() {
        nodeRoomSprites = new Dictionary<int, Sprite>();

        int loadedCount = 0;
        int resourceLoadedCount = 0;

        // Load sprites for nodes 1-21
        for (int nodeID = 1; nodeID <= TotalNodes; nodeID++) {
            int arrayIndex = nodeID - 1; // Array is 0-indexed

            Sprite nodeSprite = null;

            // Special handling for boss node (Node 21)
            if (nodeID == BossNodeID && bossNodeSprite != null) {
                nodeSprite = bossNodeSprite;
                loadedCount++;
            }
            // Check if manually assigned in Inspector
            else if (arrayIndex < nodeSprites.Length && nodeSprites[arrayIndex] != null) {
                nodeSprite = nodeSprites[arrayIndex];
                loadedCount++;
            }
            // Otherwise, try to load from Resources
            else if (!string.IsNullOrEmpty(roomImageResourcePath)) {
                string resourcePath = $"{roomImageResourcePath}/Node_{nodeID:D2}";
                nodeSprite = Resources.Load<Sprite>(resourcePath);

                if (nodeSprite != null) {
                    nodeSprites[arrayIndex] = nodeSprite; // Cache it
                    resourceLoadedCount++;
                    loadedCount++;

                    if (showDebugInfo) {
                        Debug.Log($"MapManager: Loaded sprite for Node {nodeID} from Resources at '{resourcePath}'");
                    }
                }
                else {
                    Debug.LogWarning($"MapManager: Could not load sprite for Node {nodeID} from Resources path '{resourcePath}'. " +
                                   $"Please assign it manually in the Inspector or place the sprite at 'Resources/{resourcePath}'");
                }
            }

            // Add to dictionary if valid
            if (nodeSprite != null) {
                nodeRoomSprites[nodeID] = nodeSprite;
            }
            else {
                Debug.LogWarning($"MapManager: No sprite assigned for Node {nodeID}!");
            }
        }

        Debug.Log($"MapManager: Loaded {loadedCount} node sprites ({loadedCount - resourceLoadedCount} from Inspector, {resourceLoadedCount} from Resources)");

        if (bossNodeSprite != null) {
            Debug.Log($"MapManager: Boss sprite assigned for Node {BossNodeID}");
        }
    }

    /// <summary>
    /// Assign nodes to clusters (emotional zones)
    /// </summary>
    private void AssignNodesToCluster() {
        clusterMapping = new Dictionary<int, List<int>>();
        nodeToCluster = new Dictionary<int, int>();

        // Initialize cluster lists
        for (int i = 0; i < ClusterCount; i++) {
            clusterMapping[i] = new List<int>();
        }

        // Distribute nodes across 5 emotional clusters
        // Cluster 0 (gioia/joy): nodes 1-4
        // Cluster 1 (rabbia/anger): nodes 5-8
        // Cluster 2 (disgusto/disgust): nodes 9-12
        // Cluster 3 (tristezza/sadness): nodes 13-16
        // Cluster 4 (paura/fear): nodes 17-21

        AssignNodeToCluster(1, 0);
        AssignNodeToCluster(2, 0);
        AssignNodeToCluster(3, 0);
        AssignNodeToCluster(4, 0);

        AssignNodeToCluster(5, 1);
        AssignNodeToCluster(6, 1);
        AssignNodeToCluster(7, 1);
        AssignNodeToCluster(8, 1);

        AssignNodeToCluster(9, 2);
        AssignNodeToCluster(10, 2);
        AssignNodeToCluster(11, 2);
        AssignNodeToCluster(12, 2);

        AssignNodeToCluster(13, 3);
        AssignNodeToCluster(14, 3);
        AssignNodeToCluster(15, 3);
        AssignNodeToCluster(16, 3);

        AssignNodeToCluster(17, 4);
        AssignNodeToCluster(18, 4);
        AssignNodeToCluster(19, 4);
        AssignNodeToCluster(20, 4);
        AssignNodeToCluster(21, 4); // Central hub node (Boss Node)

        Debug.Log($"MapManager: Assigned {TotalNodes} nodes across {ClusterCount} clusters");
    }

    private void AssignNodeToCluster(int nodeID, int clusterID) {
        clusterMapping[clusterID].Add(nodeID);
        nodeToCluster[nodeID] = clusterID;
    }
    #endregion

    #region Public API - Node Navigation
    /// <summary>
    /// Get the list of connected node IDs for a specific node
    /// </summary>
    public List<int> GetConnectedNodes(int nodeID) {
        if (nodeConnections.ContainsKey(nodeID)) {
            return new List<int>(nodeConnections[nodeID]); // Return copy
        }

        Debug.LogWarning($"MapManager: Node {nodeID} not found in configuration");
        return new List<int>();
    }

    /// <summary>
    /// Get the number of doors (connections) for a specific node
    /// </summary>
    public int GetDoorCount(int nodeID) {
        if (nodeDoorCounts.ContainsKey(nodeID)) {
            return nodeDoorCounts[nodeID];
        }

        Debug.LogWarning($"MapManager: Node {nodeID} not found");
        return 0;
    }

    /// <summary>
    /// Get the room background sprite for a specific node
    /// </summary>
    public Sprite GetRoomSprite(int nodeID) {
        if (nodeRoomSprites.ContainsKey(nodeID)) {
            return nodeRoomSprites[nodeID];
        }

        Debug.LogWarning($"MapManager: No room sprite found for node {nodeID}");
        return null;
    }

    /// <summary>
    /// Check if two nodes are directly connected
    /// </summary>
    public bool AreNodesConnected(int nodeID1, int nodeID2) {
        if (nodeConnections.ContainsKey(nodeID1)) {
            return nodeConnections[nodeID1].Contains(nodeID2);
        }
        return false;
    }

    /// <summary>
    /// Check if the specified node is the boss node
    /// </summary>
    public bool IsBossNode(int nodeID) {
        return nodeID == BossNodeID;
    }

    /// <summary>
    /// Called when player enters a new node through a door
    /// Automatically activates the new node's sprite and deactivates the previous one
    /// </summary>
    public void OnEnterNode(int nodeID) {
        if (!nodeConnections.ContainsKey(nodeID)) {
            Debug.LogError($"MapManager: Invalid node ID {nodeID}");
            return;
        }

        // Store previous node
        previousNodeID = currentNodeID;
        currentNodeID = nodeID;

        // Activate the sprite for the new node
        ActivateNodeSprite(nodeID);

        // Check if cluster changed
        int newClusterID = GetClusterForNode(nodeID);
        if (newClusterID != currentClusterID && newClusterID != -1) {
            OnEnterCluster(newClusterID);
        }

        string nodeType = IsBossNode(nodeID) ? " (BOSS NODE)" : "";
        Debug.Log($"MapManager: Entered node {nodeID}{nodeType} with {GetDoorCount(nodeID)} doors (from node {previousNodeID})");
    }

    /// <summary>
    /// Transition from current node to a target node through a door
    /// Validates the connection before transitioning
    /// </summary>
    public bool TransitionToNode(int targetNodeID) {
        // Validate connection
        if (currentNodeID == -1) {
            Debug.LogWarning("MapManager: No current node set. Use OnEnterNode() to set initial node.");
            OnEnterNode(targetNodeID);
            return true;
        }

        if (!AreNodesConnected(currentNodeID, targetNodeID)) {
            Debug.LogError($"MapManager: Cannot transition from node {currentNodeID} to {targetNodeID} - nodes are not connected!");
            return false;
        }

        // Perform transition
        OnEnterNode(targetNodeID);
        return true;
    }

    /// <summary>
    /// Get the current node ID the player is in
    /// </summary>
    public int GetCurrentNodeID() {
        return currentNodeID;
    }

    /// <summary>
    /// Get the previous node ID
    /// </summary>
    public int GetPreviousNodeID() {
        return previousNodeID;
    }
    #endregion

    #region Sprite Management
    /// <summary>
    /// Activate the sprite for a specific node
    /// Deactivates the previous node's sprite automatically
    /// </summary>
    private void ActivateNodeSprite(int nodeID) {
        if (roomSpriteRenderer == null) {
            Debug.LogError("MapManager: RoomSpriteRenderer is not assigned!");
            return;
        }

        Sprite targetSprite = GetRoomSprite(nodeID);

        if (targetSprite == null) {
            Debug.LogWarning($"MapManager: Cannot activate sprite for node {nodeID} - sprite not found!");
            return;
        }

        // Stop any active transition
        if (activeTransition != null) {
            StopCoroutine(activeTransition);
            activeTransition = null;
        }

        // Apply sprite transition
        if (enableSpriteTransition && previousNodeID != -1) {
            activeTransition = StartCoroutine(TransitionSpriteCoroutine(targetSprite));
        }
        else {
            // Instant sprite change
            roomSpriteRenderer.sprite = targetSprite;
            roomSpriteRenderer.enabled = true;
        }

        if (showDebugInfo) {
            Debug.Log($"MapManager: Activated sprite for Node {nodeID}");
        }
    }

    /// <summary>
    /// Coroutine for smooth sprite transition with fade effect
    /// </summary>
    private System.Collections.IEnumerator TransitionSpriteCoroutine(Sprite newSprite) {
        Color originalColor = roomSpriteRenderer.color;
        float elapsed = 0f;

        // Fade out
        while (elapsed < transitionDuration / 2f) {
            elapsed += Time.deltaTime;
            float t = elapsed / (transitionDuration / 2f);
            float alpha = Mathf.Lerp(1f, 0f, transitionCurve.Evaluate(t));
            roomSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Switch sprite
        roomSpriteRenderer.sprite = newSprite;

        elapsed = 0f;

        // Fade in
        while (elapsed < transitionDuration / 2f) {
            elapsed += Time.deltaTime;
            float t = elapsed / (transitionDuration / 2f);
            float alpha = Mathf.Lerp(0f, 1f, transitionCurve.Evaluate(t));
            roomSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure final state
        roomSpriteRenderer.color = originalColor;
        activeTransition = null;
    }
    #endregion

    #region Public API - Cluster Management
    /// <summary>
    /// Get the cluster ID that a specific node belongs to
    /// </summary>
    public int GetClusterForNode(int nodeID) {
        if (nodeToCluster.ContainsKey(nodeID)) {
            return nodeToCluster[nodeID];
        }
        return -1;
    }

    /// <summary>
    /// Get all nodes in a specific cluster
    /// </summary>
    public List<int> GetNodesInCluster(int clusterID) {
        if (clusterMapping.ContainsKey(clusterID)) {
            return new List<int>(clusterMapping[clusterID]); // Return copy
        }
        return new List<int>();
    }

    /// <summary>
    /// Called when player enters a new cluster (emotional zone)
    /// </summary>
    public void OnEnterCluster(int clusterID) {
        if (clusterID < 0 || clusterID >= ClusterCount) {
            Debug.LogError($"MapManager: Invalid cluster ID {clusterID}");
            return;
        }

        if (currentClusterID != clusterID) {
            currentClusterID = clusterID;

            // Play the corresponding cluster audio
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayClusterAudio(clusterID, useCrossfade: true);
            }

            Debug.Log($"MapManager: Entered cluster {clusterID} ({AudioManager.Instance?.GetTrackName(clusterID) ?? "Unknown"})");
        }
    }

    /// <summary>
    /// Get the current cluster ID the player is in
    /// </summary>
    public int GetCurrentClusterID() {
        return currentClusterID;
    }
    #endregion

    #region Debug
    /// <summary>
    /// Log complete map configuration for debugging
    /// </summary>
    private void LogMapConfiguration() {
        Debug.Log("=== MAP CONFIGURATION ===");
        Debug.Log($"Total Nodes: {TotalNodes}");
        Debug.Log($"Boss Node: {BossNodeID}");
        Debug.Log($"Sprite Renderer Assigned: {roomSpriteRenderer != null}");
        Debug.Log($"Boss Sprite Assigned: {bossNodeSprite != null}");

        Debug.Log("Cluster Assignments:");
        for (int i = 0; i < ClusterCount; i++) {
            List<int> nodes = GetNodesInCluster(i);
            string trackName = AudioManager.Instance?.GetTrackName(i) ?? "Unknown";
            Debug.Log($"  Cluster {i} ({trackName}): {string.Join(", ", nodes)}");
        }

        Debug.Log("Node Sprite Status:");
        int assignedSprites = 0;
        for (int i = 1; i <= TotalNodes; i++) {
            if (nodeRoomSprites.ContainsKey(i) && nodeRoomSprites[i] != null) {
                assignedSprites++;
            }
        }
        Debug.Log($"  {assignedSprites}/{TotalNodes} node sprites assigned");
    }

    /// <summary>
    /// Validate map configuration integrity
    /// </summary>
    [ContextMenu("Validate Map Configuration")]
    public void ValidateMapConfiguration() {
        Debug.Log("=== VALIDATING MAP CONFIGURATION ===");

        bool isValid = true;

        // Check SpriteRenderer
        if (roomSpriteRenderer == null) {
            Debug.LogError("VALIDATION ERROR: RoomSpriteRenderer is not assigned!");
            isValid = false;
        }

        // Check boss sprite
        if (bossNodeSprite == null) {
            Debug.LogWarning($"VALIDATION WARNING: Boss sprite for Node {BossNodeID} is not assigned!");
        }

        // Check bidirectional connections
        foreach (var kvp in nodeConnections) {
            int nodeID = kvp.Key;
            foreach (int connectedID in kvp.Value) {
                if (!nodeConnections.ContainsKey(connectedID) || !nodeConnections[connectedID].Contains(nodeID)) {
                    Debug.LogError($"VALIDATION ERROR: Connection {nodeID} -> {connectedID} is not bidirectional!");
                    isValid = false;
                }
            }
        }

        // Check if all nodes are assigned to clusters
        for (int i = 1; i <= TotalNodes; i++) {
            if (!nodeToCluster.ContainsKey(i)) {
                Debug.LogError($"VALIDATION ERROR: Node {i} is not assigned to any cluster!");
                isValid = false;
            }
        }

        // Check if all nodes have room sprites
        for (int i = 1; i <= TotalNodes; i++) {
            if (!nodeRoomSprites.ContainsKey(i) || nodeRoomSprites[i] == null) {
                Debug.LogWarning($"VALIDATION WARNING: Node {i} does not have a sprite assigned!");
            }
        }

        if (isValid) {
            Debug.Log("✓ Map configuration is valid!");
        }
        else {
            Debug.LogError("✗ Map configuration has errors!");
        }
    }
    #endregion
}