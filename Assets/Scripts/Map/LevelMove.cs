using UnityEngine;

public class LevelMove : MonoBehaviour
{
    public GameObject levelName;
    public GameObject currentLevelName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (EmotionPowerManager.Instance.AreAllPowersCollected())
            {
                levelName = MapManager.Instance.bossLevel;
                GameManager.Instance.LaunchEnableObjectForSecondsCoroutine(MapManager.Instance.bossStart, 7f);
            }
            print("Cambio Livello a " + levelName);

            levelName.SetActive(true);
            PlayerController player = FindAnyObjectByType<PlayerController>();
            player.transform.position = new Vector3(0, 0, -1);

            currentLevelName.SetActive(false);
        }
    }
}