using UnityEngine;

public class EmotionPower : MonoBehaviour
{
    public bool collected = false;
    public bool active = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;
            active = true;
            Debug.Log("Emotion Power Collected! Player abilities enhanced.");

            gameObject.SetActive(false);
        }
    }
}
