using UnityEngine;
using static Emotions;

public class Player : MonoBehaviour
{
    public int maxLife = 100;

    private int currentLife;
    private PlayerController controller;

    public Emotion Emotion { get; set; } = Emotion.Impulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLife = maxLife;
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(int damage)
    {
        currentLife -= damage;
        if (currentLife <= 0)
        {
            Application.Quit();
        }
    }
}
