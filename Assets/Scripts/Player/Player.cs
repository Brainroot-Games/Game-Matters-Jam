using UnityEngine;
using static Emotions;

public class Player : MonoBehaviour
{
    private const int maxLife = 100;
    private const int minLife = 0;
    public GameObject GameOverObj;
    public static int currentLife;
    private PlayerController controller;

    public Emotion Emotion { get; set; } = Emotion.Impulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLife = maxLife;
        controller = GetComponent<PlayerController>();
    }

    public void GetDamage(int damage)
    {
        currentLife = Mathf.Clamp(currentLife - damage, minLife, maxLife);
        if (currentLife <= 0)
        {
            EnableGameOver();
        }
    }

    private void EnableGameOver()
    {
        MapManager.gameOver = true;
        controller.enabled = false;
        MapManager.Instance.DisableLifeObj();
        GameOverObj.SetActive(true);
    }
}
