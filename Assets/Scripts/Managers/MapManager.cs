using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the brain-shaped map containing color-coded clusters and circular sub-dungeons
/// with predefined node connections and door configurations
/// </summary>
public class MapManager : MonoBehaviour {

    public Neuron firstNeuron;
    public GameObject bossLevel;
    public GameObject bossStart;
    public GameObject credits;
    public GameObject currentLifeObj;
    public Text currentLifeText;
    public static bool gameOver = false;

    private Player player;
    private Neuron currentNeuron;

    public static MapManager Instance { get; private set; }

    private void Awake() {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(transform.parent);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        currentNeuron = firstNeuron;

        EnableLifeUI();
    }

    private void Update() {
        ShowCurrentLife();
    }

    private void ShowCurrentLife()
    {
        currentLifeText.text = player.CurrentLife.ToString();
    }

    public void EnableLifeUI()
    {
        if (currentLifeObj == null) {
            Debug.LogWarning("Current life object is not assigned in MapManager.");
            return;
        }
        currentLifeObj.SetActive(true);
        currentLifeText = currentLifeObj.GetComponentInChildren<Text>();
    }

    public void DisableLifeUI() => currentLifeObj.SetActive(false);

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.LaunchEnableForSecondsThenQuitCoroutine(credits, 7f);
    }

    public void OnPlayerInSynapse(Synapse synapse)
    {
        currentNeuron = synapse.ToNeuron;
        currentNeuron.SetSpawnerActive(true);
    }

    public void OnPlayerOutSynapse(Synapse synapse)
    {
        synapse.FromNeuron.SetSpawnerActive(false);
    }
}
