using UnityEngine;

public class EmotionPower : MonoBehaviour
{
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
            StartCoroutine(EnableForSeconds(textToEnable, secondsToWait));
            
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private System.Collections.IEnumerator EnableForSeconds(GameObject obj, float seconds)
    {
        obj.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        Time.timeScale = 1;
        obj.SetActive(false);
    }
}
