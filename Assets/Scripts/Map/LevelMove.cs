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
                MapManager.Instance.StartCoroutine(EnableForSeconds(MapManager.Instance.bossStart, 7f));
            }
            print("Cambio Livello a " + levelName);

            levelName.SetActive(true);
            PlayerController player = FindAnyObjectByType<PlayerController>();
            //float x = (player.transform.position.x * -1) + (float)(this.transform.position.x > 0 ? -0.5 : 0.5);
            //float y = (player.transform.position.y * -1) + (float)(this.transform.position.y > 0 ? 0.5 : -0.5);
            player.transform.position = new Vector3(0, 0, -1);

            //Transform new_player_transform = levelName.transform.Find("LevelMove" + currentLevelName.name.Substring(2));

            //new_player_transform.gameObject.SetActive(false);

            //player.transform.position = new Vector3(new_player_transform.position.x, new_player_transform.position.y, player.transform.position.z);


            currentLevelName.SetActive(false);
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
}
