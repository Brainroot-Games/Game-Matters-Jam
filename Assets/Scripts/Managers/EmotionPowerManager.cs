using UnityEngine;

public class EmotionPowerManager : MonoBehaviour
{
    public static bool[] activePowers = new bool[6];
    public GameObject startObj;
    [SerializeField] private float secondsToWait = 7f;

    public static EmotionPowerManager Instance { get; private set; }

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
    }

    private void Start()
    {
        StartCoroutine(EnableForSeconds(startObj, secondsToWait));
    }

    public void CollectEmotionPower(Emotions.Emotion emotion)
    {
        int index = (int)emotion;
        if (index >= 0 && index < activePowers.Length)
        {
            activePowers[index] = true;
        }
    }

    private System.Collections.IEnumerator EnableForSeconds(GameObject obj, float seconds) {
        obj.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        Time.timeScale = 1;
        obj.SetActive(false);
    }
    public bool AreAllPowersCollected()
    {
        foreach (bool power in activePowers)
        {
            if (!power)
            {
                return false;
            }
        }
        return true;
    }
}
