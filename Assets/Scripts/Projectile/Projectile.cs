using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public TagHandle targetTag;

    protected Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public Emotion Emotion { get; set; } = Emotion.Impulse;
    public Vector2 Direction { get; set; } = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = GetColor(Emotion);
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + Direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    protected abstract void OnTarget(GameObject target);

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            OnTarget(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            Projectile otherProjectile = other.GetComponent<Projectile>();
            if (!otherProjectile.GetType().IsEquivalentTo(GetType()))
            {
                if (otherProjectile.Emotion.Equals(Emotion) ||
                    otherProjectile.Emotion.Equals(Emotion.Impulse) ||
                    Emotion.Equals(Emotion.Impulse))
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bounds"))
        {
            Destroy(gameObject);
        }
    }
}
