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
    public float moveAnimationTime = 0.2f;

    private Player player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private float actualSpeed = 0f;
    private bool canMove = true;
    private bool canAttack = true;

    public bool CanMove { get => canMove; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        actualSpeed = speed;

        StartCoroutine(MoveAnimation());
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (CanMove)
        {
            direction = moveInput;
        }
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

            player.AddProjectile(projectile);

            StartCoroutine(AttackCooldown());
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (TryGetEmotionFromInput(context.control.name, out Emotion emotion))
        {
            if (EmotionPowerManager.Instance.activePowers[GetEmotionIndex(emotion)])
                player.Emotion = emotion;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        player.Emotion = Emotion.Neutral;
    }

    public void Move()
    {
        Vector2 newPosition = rb.position + actualSpeed * Time.deltaTime * direction;
        rb.MovePosition(newPosition);
    }

    public void AutoMove(Vector2 from, Vector2 to, float moveTime)
    {
        canMove = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = from;
        direction = (to - from).normalized;
        actualSpeed = Utils.GetSpeed(from, to, moveTime);

        StartCoroutine(MoveCooldown(moveTime, to));
    }

    private IEnumerator MoveCooldown(float moveTime, Vector2 finalPosition)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(moveTime);
        yield return new WaitForFixedUpdate();

        ResumeManualMove(finalPosition);
    }

    private void ResumeManualMove(Vector2 position)
    {
        transform.position = position;
        rb.bodyType = RigidbodyType2D.Dynamic;
        direction = moveInput;
        actualSpeed = speed;
        canMove = true;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator MoveAnimation()
    {
        while (true)
        {
            if (direction != Vector2.zero)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            yield return new WaitForSeconds(moveAnimationTime);
        }
    }

    private void OnValidate()
    {
        speed = Mathf.Max(0, speed);
        attackCooldown = Mathf.Max(0, attackCooldown);
    }
}
