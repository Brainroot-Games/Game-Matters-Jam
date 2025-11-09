using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject projectilePrefab;
    public float attackCooldown = 1f;

    private Player player;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastPosition;
    private bool canAttack = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        lastPosition = rb.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        lastPosition = rb.position;
        Vector2 newPosition = rb.position + movement * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            PlayerProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                .GetComponent<PlayerProjectile>();
            projectile.Emotion = player.Emotion;
            projectile.Direction =
                (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - projectile.transform.position).normalized;

            StartCoroutine(AttackCooldown());
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (System.Enum.TryParse<Emotion>(context.control.name, out var emotion))
        {
            player.Emotion = emotion;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        player.Emotion = Emotion.Impulse;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bounds"))
        {
            rb.MovePosition(lastPosition);
        }
    }
}
