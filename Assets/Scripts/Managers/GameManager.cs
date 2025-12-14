using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public event Action OnPause;
    public event Action OnResume;
    public event Action OnEscapePressed;

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

    private void Start() {
        DisableObject(pauseMenuUI);
    }

    private void Update() {
        if (isGamePaused) {
            MapManager.Instance.DisableLifeObj();
            } else {
            MapManager.Instance.EnableLifeObj();
        }
    }

    public static bool IsPaused()  =>  isGamePaused;

    public void PauseGame() {
        if (IsPaused()) return;
        
        OnPause?.Invoke();
        isGamePaused = true;
        EnableObject(pauseMenuUI);
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        if (!IsPaused()) return;

        OnResume?.Invoke();
        isGamePaused = false;
        DisableObject(pauseMenuUI);
        Time.timeScale = 1f;
    }
    
    private IEnumerator EnableObjectForSeconds(GameObject obj, float seconds) {
        EnableObject(obj);

        // Pause the game
        PauseGame();

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        ResumeGame();
        DisableObject(obj);
    }

    private IEnumerator EnableForSecondsThenQuit(GameObject obj, float seconds) {
        EnableObject(obj);

        PauseGame();

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        Application.Quit();
    }

    public void LaunchEnableObjectForSecondsCoroutine(GameObject obj, float seconds) {
        StartCoroutine(EnableObjectForSeconds(obj, seconds));
    }

    public void LaunchEnableForSecondsThenQuitCoroutine(GameObject obj, float seconds) {
        StartCoroutine(EnableForSecondsThenQuit(obj, seconds));
    }

    private void ManagePauseGameByInput() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnEscapePressed?.Invoke();
            if (isGamePaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    private void EnableObject(GameObject obj) {
        obj.SetActive(true);
    }

    private void DisableObject(GameObject obj) {
        obj.SetActive(false);
    }
}