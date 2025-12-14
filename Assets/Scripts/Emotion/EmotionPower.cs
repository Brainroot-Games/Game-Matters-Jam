using UnityEngine;

public class EmotionPower : MonoBehaviour {
    public Emotions.Emotion emotion;
    public AudioClip audioClip;
    public GameObject textToEnable;
    [SerializeField] private float secondsToWait = 7f;

    public bool collected = false;
    public bool active = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collected)
        {
            collected = true;
            active = true;
            Debug.Log("Emotion Power Collected! Player abilities enhanced.");

            EmotionPowerManager.Instance.CollectEmotionPower(emotion);
            // dialog
            AudioManager.Instance.PlayAudioClip((int)emotion);
            GameManager.Instance.StartEnableObjectForSecondsCoroutine(textToEnable, secondsToWait);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
