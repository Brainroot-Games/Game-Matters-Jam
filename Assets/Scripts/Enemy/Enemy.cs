using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    public int maxLife = 30;
    public int damage = 20;
    public float speed = 2.0f;
    public float stopAt = 0.1f;
    private float sqrStopAt;
    public GameObject projectilePrefab;

    protected int currentLife;
    protected Vector2 targetPosition;
    protected Transform player;

    protected SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public Emotion Emotion { get; set; } = Emotion.Neutral;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        sqrStopAt = stopAt * stopAt;
        currentLife = maxLife;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetSprite();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector2 movement = targetPosition - (Vector2)transform.position;
        rb.linearVelocity = (movement.sqrMagnitude > sqrStopAt) ?
            movement.normalized * speed :
            Vector2.zero;
    }

    public virtual void GetDamage(int damage, Emotion damageEmotion)
    {
        if (damageEmotion.Equals(Emotion))
        {
            damage = SameEmotionDamage(damage);
        }

        currentLife -= damage;

        if (currentLife <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected abstract int SameEmotionDamage(int damage);

    protected abstract void SetSprite();

    protected virtual void OnCollisionEnter2DPlayer(Player player)
    {
        player.GetDamage(damage);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnCollisionEnter2DPlayer(collision.gameObject.GetComponent<Player>());
        }
    }
}
