using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    private Rigidbody2D rb;
    private Vector2 direction;

    public Emotion Emotion { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (worldPos - rb.position).normalized;

        GetComponentInChildren<SpriteRenderer>().color = GetColor(Emotion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }
}
