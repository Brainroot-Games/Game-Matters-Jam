using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages audio playback for the 5 cluster zones in the brain-shaped map
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Cluster Audio Tracks")]
    [SerializeField] private AudioClip[] clusterAudioTracks = new AudioClip[5];

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

        // Initialize cluster to audio mapping
        clusterToAudioMapping = new Dictionary<int, AudioClip>();
        for (int i = 0; i < clusterAudioTracks.Length; i++)
        {
            if (clusterAudioTracks[i] != null)
            {
                clusterToAudioMapping[i] = clusterAudioTracks[i];
            }
            else
            {
                Debug.LogWarning($"AudioManager: Audio track for cluster {i} is not assigned!");
            }
        }
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
            Debug.LogError($"AudioManager: No audio track assigned for cluster {clusterID}");
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