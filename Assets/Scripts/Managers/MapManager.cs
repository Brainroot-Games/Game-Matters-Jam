using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// with predefined node connections and door configurations
/// </summary>
public class MapManager : MonoBehaviour {

    public GameObject bossLevel;
    public GameObject bossStart;
    public GameObject credits;

    public static MapManager Instance { get; private set; }

    private void Awake() {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    private System.Collections.IEnumerator EnableForSeconds(GameObject obj, float seconds)
    {
        obj.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        Application.Quit();
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Instance.StartCoroutine(EnableForSeconds(credits, 7f));
    }
}
