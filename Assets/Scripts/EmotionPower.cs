using UnityEngine;

public class EmotionPower : MonoBehaviour
{
    public Emotions.Emotion emotion;
    public string dialog;
    public AudioClip audioClip;

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
            AudioManager.Instance.PlayClusterAudio((int)emotion);

            gameObject.SetActive(false);
        }
    }
}
