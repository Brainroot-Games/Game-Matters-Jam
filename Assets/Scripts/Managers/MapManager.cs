using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// with predefined node connections and door configurations
/// </summary>
public class MapManager : MonoBehaviour {

    public GameObject bossLevel;
    public GameObject bossStart;
    public GameObject credits;
    public GameObject currentLifeObj;
    public Text currentLifeText;
    public static bool gameOver = false;

    public static MapManager Instance { get; private set; }

    private void Awake() {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject.transform.parent);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        EnableLifeObj();
    }

    private void Update() {
        ShowCurrentLife();
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.LaunchEnableForSecondsThenQuitCoroutine(credits, 7f);
    }

    private void ShowCurrentLife()
    {
        currentLifeText.text = Player.currentLife.ToString();
    }

    public void EnableLifeObj()
    {
        if (currentLifeObj == null) {
            Debug.LogWarning("Current life object is not assigned in MapManager.");
            return;
        }
        currentLifeText = currentLifeObj.GetComponentInChildren<Text>();
        currentLifeObj.SetActive(true);
    }

    public void DisableLifeObj()
    {
        if (gameOver)
            currentLifeObj.SetActive(false);
    }
}