using System.Collections;
using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    protected const int minLife = 0;

    public int maxLife = 30;
    public int damage = 20;
    public float speed = 2.5f;
    public float stopAt = 0.1f;
    public float moveAnimationTime = 0.2f;
    public GameObject projectilePrefab;

    protected int currentLife;
    protected Vector2 targetPosition;
    protected Player player;
    protected SpriteRenderer spriteRenderer;

    private float sqrStopAt;
    private Rigidbody2D rb;
    private bool isMoving = false;

    public Emotion Emotion { get; set; } = Emotion.Neutral;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        sqrStopAt = stopAt * stopAt;
        currentLife = maxLife;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetSprite();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        isMoving = !player.IsInSynapse;
        if (isMoving)
        {
            Move();
        }
    }

    public virtual void GetDamage(int damage, Emotion damageEmotion)
    {
        if (damageEmotion.Equals(Emotion))
        {
            damage = SameEmotionDamage(damage);
        }

        currentLife = Mathf.Clamp(currentLife - damage, minLife, maxLife);

        if (currentLife == minLife)
        {
            Die();
        }
    }

    protected abstract int SameEmotionDamage(int damage);

    protected abstract void SetSprite();

    protected virtual void Move()
    {
        Vector2 toTarget = targetPosition - rb.position;
        isMoving = toTarget.sqrMagnitude > sqrStopAt;
        if (isMoving)
        {
            Vector2 newPosition = rb.position + speed * Time.deltaTime * toTarget.normalized;
            rb.MovePosition(newPosition);
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator MoveAnimation()
    {
        while (true)
        {
            if (isMoving)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            yield return new WaitForSeconds(moveAnimationTime);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(MoveAnimation());
    }

    protected virtual void OnPlayerCollisionEnter(Player player)
    {
        player.GetDamage(damage);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (!player.IsInSynapse)
            {
                OnPlayerCollisionEnter(player);
            }
        }
    }

    protected virtual void OnValidate()
    {
        maxLife = Mathf.Max(minLife, maxLife);
        damage = Mathf.Max(0, damage);
        speed = Mathf.Max(0, speed);
        stopAt = Mathf.Max(0, stopAt);
    }
}
