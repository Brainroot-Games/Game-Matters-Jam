using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Collider2D))]
public class EmotionPower : MonoBehaviour {
    public Emotion emotion;
    public AudioClip audioClip;
    public GameObject textToEnable;
    [SerializeField] private float secondsToWait = 7f;

    public bool collected = false;
    public bool active = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;
            active = true;

            EmotionPowerManager.Instance.CollectEmotionPower(emotion);
            AudioManager.Instance.PlayAudioClip(GetEmotionIndex(emotion));
            GameManager.Instance.LaunchEnableObjectForSecondsCoroutine(textToEnable, secondsToWait);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
