using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages audio playback for the 5 cluster zones in the brain-shaped map
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Neutral Audio (Default)")]
    [SerializeField] private AudioClip neutralAudioClip;
    
    [Header("Cluster Audio Tracks")]
    [SerializeField] private AudioClip[] clusterAudioTracks = new AudioClip[5];
    
    [Header("Track Names (for reference)")]
    [SerializeField] private string[] trackNames = new string[] { "gioia", "rabbia", "disgusto", "tristezza", "paura" };
    
    [Header("Audio Resources Path")]
    [Tooltip("Path to audio files in Resources folder (leave empty if assigning manually in Inspector)")]
    [SerializeField] private string audioResourcePath = "Audio/Clusters";

    [Header("Audio Source Settings")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] private float crossfadeDuration = 2f;

    private int currentClusterID = -1;
    private Dictionary<int, AudioClip> clusterToAudioMapping;
    private Coroutine crossfadeCoroutine;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAudioManager();
    }

    private void InitializeAudioManager()
    {
        // Create AudioSource if not assigned
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.volume = masterVolume;

        // Load audio clips from Resources if not manually assigned
        LoadAudioClips();

        // Initialize cluster to audio mapping
        clusterToAudioMapping = new Dictionary<int, AudioClip>();
        for (int i = 0; i < clusterAudioTracks.Length; i++)
        {
            if (clusterAudioTracks[i] != null)
            {
                clusterToAudioMapping[i] = clusterAudioTracks[i];
                Debug.Log($"AudioManager: Cluster {i} assigned track '{trackNames[i]}' - {clusterAudioTracks[i].name}");
            }
            else
            {
                Debug.LogWarning($"AudioManager: Audio track for cluster {i} ({trackNames[i]}) is not assigned!");
            }
        }

        // Play neutral audio clip at start
        PlayNeutralAudio();
    }

    /// <summary>
    /// Load audio clips from Resources folder based on track names
    /// </summary>
    private void LoadAudioClips()
    {
        // Try to load neutral audio clip if not assigned
        if (neutralAudioClip == null && !string.IsNullOrEmpty(audioResourcePath))
        {
            string neutralPath = $"{audioResourcePath}/neutral";
            neutralAudioClip = Resources.Load<AudioClip>(neutralPath);
            
            if (neutralAudioClip != null)
            {
                Debug.Log($"AudioManager: Loaded 'neutral' audio from Resources at '{neutralPath}'");
            }
            else
            {
                Debug.LogWarning($"AudioManager: Could not load neutral audio clip from Resources path '{neutralPath}'. " +
                               $"Please assign it manually in the Inspector or place the audio file at 'Resources/{neutralPath}'");
            }
        }

        // Only load from Resources if clips are not manually assigned
        bool needsLoading = false;
        for (int i = 0; i < clusterAudioTracks.Length; i++)
        {
            if (clusterAudioTracks[i] == null)
            {
                needsLoading = true;
                break;
            }
        }

        if (!needsLoading)
        {
            Debug.Log("AudioManager: All audio clips are manually assigned in Inspector.");
            return;
        }

        // Try to load from Resources folder
        if (!string.IsNullOrEmpty(audioResourcePath))
        {
            for (int i = 0; i < trackNames.Length; i++)
            {
                if (clusterAudioTracks[i] == null)
                {
                    string resourcePath = $"{audioResourcePath}/{trackNames[i]}";
                    AudioClip clip = Resources.Load<AudioClip>(resourcePath);
                    
                    if (clip != null)
                    {
                        clusterAudioTracks[i] = clip;
                        Debug.Log($"AudioManager: Loaded '{trackNames[i]}' from Resources at '{resourcePath}'");
                    }
                    else
                    {
                        Debug.LogWarning($"AudioManager: Could not load audio clip '{trackNames[i]}' from Resources path '{resourcePath}'. " +
                                       $"Please either assign it manually in the Inspector or place the audio file at 'Resources/{resourcePath}'");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: Audio resource path is empty. Please assign audio clips manually in the Inspector.");
        }
    }

    /// <summary>
    /// Play the default neutral audio track
    /// </summary>
    public void PlayNeutralAudio(bool useCrossfade = false)
    {
        if (neutralAudioClip == null)
        {
            Debug.LogWarning("AudioManager: Neutral audio clip is not assigned!");
            return;
        }

        if (useCrossfade && musicAudioSource.isPlaying)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }
            crossfadeCoroutine = StartCoroutine(CrossfadeToClip(neutralAudioClip));
        }
        else
        {
            musicAudioSource.clip = neutralAudioClip;
            musicAudioSource.Play();
        }

        currentClusterID = -1;
        Debug.Log("AudioManager: Now playing neutral audio");
    }

    /// <summary>
    /// Get the name of the track for a specific cluster
    /// </summary>
    public string GetTrackName(int clusterID)
    {
        if (clusterID == -1)
        {
            return "neutral";
        }
        if (clusterID >= 0 && clusterID < trackNames.Length)
        {
            return trackNames[clusterID];
        }
        return "Unknown";
    }

    /// <summary>
    /// Play the audio track associated with a specific cluster
    /// </summary>
    /// <param name="clusterID">ID of the cluster (0-4)</param>
    /// <param name="useCrossfade">Whether to crossfade between tracks</param>
    public void PlayClusterAudio(int clusterID, bool useCrossfade = true)
    {
        if (clusterID < 0 || clusterID >= 5)
        {
            Debug.LogError($"AudioManager: Invalid cluster ID {clusterID}. Must be between 0 and 4.");
            return;
        }

        if (currentClusterID == clusterID)
        {
            return; // Already playing this cluster's audio
        }

        if (!clusterToAudioMapping.ContainsKey(clusterID))
        {
            Debug.LogError($"AudioManager: No audio track assigned for cluster {clusterID} ({trackNames[clusterID]})");
            return;
        }

        AudioClip targetClip = clusterToAudioMapping[clusterID];

        if (useCrossfade && musicAudioSource.isPlaying)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }
            crossfadeCoroutine = StartCoroutine(CrossfadeToClip(targetClip));
        }
        else
        {
            musicAudioSource.clip = targetClip;
            musicAudioSource.Play();
        }

        currentClusterID = clusterID;
        Debug.Log($"AudioManager: Now playing cluster {clusterID} - '{trackNames[clusterID]}'");
    }

    /// <summary>
    /// Crossfade from current track to a new track
    /// </summary>
    private System.Collections.IEnumerator CrossfadeToClip(AudioClip newClip)
    {
        float startVolume = musicAudioSource.volume;
        float elapsed = 0f;

        // Fade out current track
        while (elapsed < crossfadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (crossfadeDuration / 2f));
            yield return null;
        }

        // Switch to new clip
        musicAudioSource.clip = newClip;
        musicAudioSource.Play();

        elapsed = 0f;

        // Fade in new track
        while (elapsed < crossfadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, masterVolume, elapsed / (crossfadeDuration / 2f));
            yield return null;
        }

        musicAudioSource.volume = masterVolume;
        crossfadeCoroutine = null;
    }

    /// <summary>
    /// Stop the currently playing audio
    /// </summary>
    public void StopAudio()
    {
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
            crossfadeCoroutine = null;
        }

        musicAudioSource.Stop();
        currentClusterID = -1;
    }

    /// <summary>
    /// Pause the currently playing audio
    /// </summary>
    public void PauseAudio()
    {
        musicAudioSource.Pause();
    }

    /// <summary>
    /// Resume the paused audio
    /// </summary>
    public void ResumeAudio()
    {
        musicAudioSource.UnPause();
    }

    /// <summary>
    /// Set the master volume for all audio
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = masterVolume;
    }

    /// <summary>
    /// Get the currently playing cluster ID
    /// </summary>
    public int GetCurrentClusterID()
    {
        return currentClusterID;
    }

    /// <summary>
    /// Check if audio is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return musicAudioSource.isPlaying;
    }

    /// <summary>
    /// Set the crossfade duration
    /// </summary>
    public void SetCrossfadeDuration(float duration)
    {
        crossfadeDuration = Mathf.Max(0.1f, duration);
    }
}