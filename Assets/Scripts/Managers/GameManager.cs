using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    static bool isGamePaused = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }
    public static bool IsPaused()  =>  isGamePaused;

    public void PauseGame() {
        if (isGamePaused) return;

        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        if (!isGamePaused) return;

        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    private IEnumerator EnableObjectForSeconds(GameObject obj, float seconds) {
        obj.SetActive(true);

        // Pause the game
        PauseGame();

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        ResumeGame();
        obj.SetActive(false);
    }

    public void StartEnableObjectForSecondsCoroutine(GameObject obj, float seconds) {
        StartCoroutine(EnableObjectForSeconds(obj, seconds));
    }
}