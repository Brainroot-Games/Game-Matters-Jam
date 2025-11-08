using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    private Vector2 direction;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (worldPos - rb.position).normalized;
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
