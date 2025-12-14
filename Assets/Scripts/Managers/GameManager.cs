using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject GameOverUI;
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
        // Ensure pause menu is disabled at start (defensive in case inspector changed)
        if (pauseMenuUI != null) {
            DisableObject(pauseMenuUI);
        }
    }

    private void Update() {
        CheckEscapePressed();

        // Handle life UI based on pause state (guard against missing MapManager)
        if (MapManager.Instance != null) {
            if (IsPaused()) {
                MapManager.Instance.DisableLifeUI();
            }
            else {
                MapManager.Instance.EnableLifeUI();
            }
        }
    }

    public static bool IsPaused() => isGamePaused;

    public void PauseGame(bool isMenuPause) {
        if (IsPaused()) return;

        if (isMenuPause) {
            OnPause?.Invoke();

            if (pauseMenuUI != null) {
                EnableObject(pauseMenuUI);
            }
        }

        SetGamePaused();
        Time.timeScale = 0f;
    }

    public void ResumeGame(bool isMenuPause) {
        if (!IsPaused()) return;

        if (isMenuPause) {
            OnResume?.Invoke();

            if (pauseMenuUI != null) {
                DisableObject(pauseMenuUI);
            }
        }

        UnsetGamePaused();
        Time.timeScale = 1f;
    }

    private void CheckEscapePressed() {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // Use new Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            OnEscapePressed?.Invoke();
            if (IsPaused()) {
                ResumeGame(true);
            }
            else {
                PauseGame(true);
            }
        }
#else
        // Fallback to legacy Input
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnEscapePressed?.Invoke();
            if (IsPaused()) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
#endif
    }

    private IEnumerator EnableObjectForSeconds(GameObject obj, float seconds) {
        EnableObject(obj);

        // Pause the game
        PauseGame(false);

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        ResumeGame(false);
        DisableObject(obj);
    }

    private IEnumerator EnableForSecondsThenQuit(GameObject obj, float seconds) {
        EnableObject(obj);

        PauseGame(false);

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

    public static void EnableObject(GameObject obj) {
        if (obj != null) {
            obj.SetActive(true);
        }
    }

    public static void DisableObject(GameObject obj) {
        if (obj != null) {
            obj.SetActive(false);
        }
    }

    private void SetGamePaused() {
        if (IsPaused()) return;

        isGamePaused = true;
    }

    private void UnsetGamePaused() {
        if (!IsPaused()) return;

        isGamePaused = false;
    }

    public void QuitApp() {
        Application.Quit();
    }
}