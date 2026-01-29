using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Min(0)]
    public float speed = 5.0f;
    public GameObject projectilePrefab;
    [Min(0)]
    public float attackCooldown = 1f;

    private Player player;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private float actualSpeed = 0f;
    private bool canMove = true;
    private bool canAttack = true;

    private bool IsMoving => direction != Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        actualSpeed = speed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (canMove)
        {
            direction = moveInput;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack && canMove)
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

        animator.SetBool("IsMoving", IsMoving);
    }

    public void AutoMove(Synapse synapse)
    {
        canMove = false;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector2 from = synapse.teleport.transform.position;
        Vector2 to = synapse.spawn.transform.position;
        transform.position = from;
        direction = (to - from).normalized;
        actualSpeed = Utils.GetSpeed(from, to, synapse.moveTime);

        StartCoroutine(MoveCooldown(synapse));
    }

    private IEnumerator MoveCooldown(Synapse synapse)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(synapse.moveTime);
        yield return new WaitForFixedUpdate();

        EventManager.Instance.onPlayerOutSynapse.Invoke(synapse);
    }

    public void ResumeManualMove(Vector2 position)
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
}
