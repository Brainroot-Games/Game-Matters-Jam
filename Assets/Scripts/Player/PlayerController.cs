using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject projectilePrefab;
    public float attackCooldown = 1f;

    private Player player;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canAttack = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
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
                ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) -
                (Vector2)projectile.transform.position).normalized;

            StartCoroutine(AttackCooldown());
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (System.Enum.TryParse<Emotion>(context.control.name, out var emotion))
        {
            if (EmotionPowerManager.Instance.activePowers[(int)emotion])
                player.Emotion = emotion;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        player.Emotion = Emotion.Neutral;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
