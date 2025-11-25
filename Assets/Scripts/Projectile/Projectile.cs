using System.Collections.Generic;
using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 10;
    public TagHandle targetTag;

    protected Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public Emotion Emotion { get; set; } = Emotion.Neutral;
    public Vector2 Direction { get; set; } = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = SpriteManager.Instance.projectileSprites[(int)Emotion];
    }

    protected virtual void FixedUpdate()
    {
        Vector2 newPosition = rb.position + Direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    public void SetEmotion(Emotion emotion)
    {
        Emotion = emotion;
        spriteRenderer.sprite = SpriteManager.Instance.projectileSprites[(int)Emotion];
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
                    Emotion.Equals(Emotion.Neutral))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
