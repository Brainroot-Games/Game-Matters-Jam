using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages audio playback for the 5 cluster zones in the brain-shaped map
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Cluster Audio Tracks")]
    [SerializeField] private AudioClip[] emotionAudioTracks = new AudioClip[6];
    
    [Header("Track Names (for reference)")]
    [SerializeField] private string[] trackNames = new string[] { "neutro", "rabbia", "tristezza", "disgusto", "gioia", "paura"};

    [Header("Audio Mixing Settings")]
    [SerializeField] private float baseTrackVolume = 1.0f;
    [SerializeField] private float layeredTrackVolume = 0.6f;

    // Dictionary to track active audio sources for each emotion
    private Dictionary<int, AudioSource> activeAudioSources = new Dictionary<int, AudioSource>();
    
    // Base audio source for the initial track
    private AudioSource baseAudioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Get the base audio source component
            baseAudioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() 
    {
        PlayAudioClip(0);
    }

    public void PlayAudioClip(int emotionID) {
        // Validate emotion ID
        if (emotionID < 0 || emotionID >= emotionAudioTracks.Length || emotionAudioTracks[emotionID] == null) {
            Debug.LogWarning($"Invalid emotion ID: {emotionID} or missing audio clip");
            return;
        }

        // If this is the first track (emotion 0), use the base audio source
        if (emotionID == 0) {
            if (baseAudioSource != null) {
                // Only set up and play if it's not already playing the same clip
                if (baseAudioSource.clip != emotionAudioTracks[emotionID] || !baseAudioSource.isPlaying) {
                    baseAudioSource.clip = emotionAudioTracks[emotionID];
                    baseAudioSource.volume = baseTrackVolume;
                    baseAudioSource.loop = true;
                    baseAudioSource.Play();
                }

                // Track this as an active source
                if (activeAudioSources.ContainsKey(emotionID)) {
                    activeAudioSources[emotionID] = baseAudioSource;
                }
                else {
                    activeAudioSources.Add(emotionID, baseAudioSource);
                }
            }
            return;
        }

        // Check if this emotion is already playing
        if (activeAudioSources.ContainsKey(emotionID) && activeAudioSources[emotionID] != null) {
            Debug.Log($"Emotion {trackNames[emotionID]} is already playing");
            return;
        }

        // Create a new AudioSource component for layering
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();

        // Configure the new audio source
        newAudioSource.clip = emotionAudioTracks[emotionID];
        newAudioSource.volume = layeredTrackVolume;
        newAudioSource.loop = true;
        newAudioSource.playOnAwake = false;

        // Match some properties with the base audio source for consistency
        if (baseAudioSource != null) {
            newAudioSource.spatialBlend = baseAudioSource.spatialBlend;
            newAudioSource.rolloffMode = baseAudioSource.rolloffMode;
            newAudioSource.maxDistance = baseAudioSource.maxDistance;
        }

        // Start playing the layered track
        newAudioSource.Play();

        // Track this audio source
        activeAudioSources.Add(emotionID, newAudioSource);

        Debug.Log($"Layered emotion track: {trackNames[emotionID]}");
    }

    /// <summary>
    /// Stops a specific emotion track
    /// </summary>
    /// <param name="emotionID">The emotion ID to stop</param>
    public void StopAudioClip(int emotionID)
    {
        if (activeAudioSources.ContainsKey(emotionID) && activeAudioSources[emotionID] != null)
        {
            AudioSource audioSource = activeAudioSources[emotionID];
            
            // Don't destroy the base audio source, just stop it
            if (emotionID == 0 && audioSource == baseAudioSource)
            {
                audioSource.Stop();
            }
            else
            {
                // Stop and destroy the layered audio source
                audioSource.Stop();
                Destroy(audioSource);
            }
            
            activeAudioSources.Remove(emotionID);
            Debug.Log($"Stopped emotion track: {trackNames[emotionID]}");
        }
    }

    /// <summary>
    /// Stops all emotion tracks except the base track (emotion 0)
    /// </summary>
    public void StopAllLayeredTracks()
    {
        List<int> tracksToRemove = new List<int>();
        
        foreach (var kvp in activeAudioSources)
        {
            if (kvp.Key != 0) // Don't stop the base track
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Stop();
                    Destroy(kvp.Value);
                }
                tracksToRemove.Add(kvp.Key);
            }
        }
        
        // Remove stopped tracks from the dictionary
        foreach (int emotionID in tracksToRemove)
        {
            activeAudioSources.Remove(emotionID);
        }
        
        Debug.Log("Stopped all layered emotion tracks");
    }

    /// <summary>
    /// Gets the volume of a specific emotion track
    /// </summary>
    /// <param name="emotionID">The emotion ID</param>
    /// <returns>Volume level or -1 if track is not active</returns>
    public float GetTrackVolume(int emotionID)
    {
        if (activeAudioSources.ContainsKey(emotionID) && activeAudioSources[emotionID] != null)
        {
            return activeAudioSources[emotionID].volume;
        }
        return -1f;
    }

    /// <summary>
    /// Sets the volume of a specific emotion track
    /// </summary>
    /// <param name="emotionID">The emotion ID</param>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetTrackVolume(int emotionID, float volume)
    {
        if (activeAudioSources.ContainsKey(emotionID) && activeAudioSources[emotionID] != null)
        {
            activeAudioSources[emotionID].volume = Mathf.Clamp01(volume);
            Debug.Log($"Set volume for {trackNames[emotionID]} to {volume}");
        }
    }

    /// <summary>
    /// Checks if a specific emotion track is currently playing
    /// </summary>
    /// <param name="emotionID">The emotion ID</param>
    /// <returns>True if the track is playing</returns>
    public bool IsTrackPlaying(int emotionID)
    {
        return activeAudioSources.ContainsKey(emotionID) && 
               activeAudioSources[emotionID] != null && 
               activeAudioSources[emotionID].isPlaying;
    }
}