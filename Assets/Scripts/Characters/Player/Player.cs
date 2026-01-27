using UnityEngine;
using static Emotions;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private const int minLife = 0;

    public int maxLife = 100;

    private PlayerController controller;

    public int CurrentLife { get; set; }
    public Emotion Emotion { get; set; } = Emotion.Neutral;
    public bool IsInSynapse { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentLife = maxLife;
        controller = GetComponent<PlayerController>();
    }

    public void GetDamage(int damage)
    {
        CurrentLife = Mathf.Clamp(CurrentLife - damage, minLife, maxLife);
        if (CurrentLife == minLife)
        {
            Die();
        }
    }

    private void Die()
    {
        EnableGameOver();
        Destroy(gameObject);
    }

    private void EnableGameOver()
    {
        MapManager.gameOver = true;
        MapManager.Instance.DisableLifeUI();
        controller.enabled = false;
        GameManager.EnableObject(GameManager.Instance.GameOverUI);
    }

    public void OnPlayerInSynapse(Synapse synapse)
    {
        IsInSynapse = true;
        controller.AutoMove(synapse);
    }

    public void OnPlayerOutSynapse(Synapse synapse)
    {
        IsInSynapse = false;
        controller.ResumeManualMove(synapse.spawn.position);
    }

    private void OnValidate()
    {
        maxLife = Mathf.Max(minLife, maxLife);
    }
}
