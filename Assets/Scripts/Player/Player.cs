using System.Collections.Generic;
using UnityEngine;
using static Emotions;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private const int minLife = 0;

    public int maxLife = 100;

    private PlayerController controller;
    private List<PlayerProjectile> projectiles = new List<PlayerProjectile>();

    public int CurrentLife { get; set; }
    public Emotion Emotion { get; set; } = Emotion.Neutral;
    public bool IsInSynapse { get => !controller.CanMove; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentLife = maxLife;
        controller = GetComponent<PlayerController>();
    }

    public void AddProjectile(PlayerProjectile projectile)
    {
        projectiles.Add(projectile);
    }

    public void RemoveProjectile(PlayerProjectile projectile)
    {
        projectiles.Remove(projectile);
    }

    public void MoveInSynapse(Transform teleport, Transform spawn, float moveTime)
    {
        projectiles.ForEach(projectile => Destroy(projectile.gameObject));
        projectiles.Clear();

        controller.AutoMove(teleport.position, spawn.position, moveTime);
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

    private void OnValidate()
    {
        maxLife = Mathf.Max(minLife, maxLife);
    }
}
