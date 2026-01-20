using UnityEngine;
using static Emotions;

public class EmotionPowerManager : MonoBehaviour
{
    public bool[] activePowers = new bool[6];
    public GameObject startObj;
    [SerializeField] private float secondsToWait = 7f;

    public static EmotionPowerManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject.transform.parent);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() => GameManager.Instance.LaunchEnableObjectForSecondsCoroutine(startObj, secondsToWait);
    
    public void CollectEmotionPower(Emotion emotion)
    {
        int index = (int)emotion;
        if (index >= 0 && index < activePowers.Length)
        {
            activePowers[index] = true;
        }
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