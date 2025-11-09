using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// with predefined node connections and door configurations
/// </summary>
public class MapManager : MonoBehaviour {
    public static MapManager Instance { get; private set; }
}