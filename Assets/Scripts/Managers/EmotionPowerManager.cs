using UnityEngine;

public class EmotionPowerManager : MonoBehaviour
{
    public bool[] activePowers = new bool[6];

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

    public void CollectEmotionPower(Emotions.Emotion emotion)
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
