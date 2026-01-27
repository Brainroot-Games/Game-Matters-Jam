using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 10;
    public TagHandle targetTag;

    protected Rigidbody2D rb;
    protected Player player;

    private SpriteRenderer spriteRenderer;

    public Emotion Emotion { get; set; } = Emotion.Neutral;
    public Vector2 Direction { get; set; } = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = SpriteManager.Instance.projectileSprites[GetEmotionIndex(Emotion)];
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetEmotion(Emotion emotion)
    {
        Emotion = emotion;
        spriteRenderer.sprite = SpriteManager.Instance.projectileSprites[GetEmotionIndex(Emotion)];
    }

    protected virtual void Move()
    {
        Vector2 newPosition = rb.position + speed * Time.deltaTime * Direction;
        rb.MovePosition(newPosition);
    }

    protected abstract void OnTargetTriggerEnter(GameObject target);

    protected virtual void OnBoundsTriggerEnter(Collider2D other)
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && !player.IsInSynapse)
        {
            OnTargetTriggerEnter(other.gameObject);
        }
        else if (other.CompareTag("Projectile"))
        {
            Projectile otherProjectile = other.GetComponent<Projectile>();
            if (!otherProjectile.GetType().IsEquivalentTo(GetType()))
            {
                if (otherProjectile.Emotion.Equals(Emotion) ||
                    Emotion.Equals(Emotion.Neutral))
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (other.CompareTag("Bounds"))
        {
            OnBoundsTriggerEnter(other);
        }
    }
}
