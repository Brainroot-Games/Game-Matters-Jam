using UnityEngine;
using static Emotions;

public abstract class Enemy : MonoBehaviour
{
    public int maxLife = 30;
    public int damage = 20;
    public float speed = 2.0f;
    public GameObject projectilePrefab;

    protected int currentLife;
    protected Vector3 targetPosition;
    protected GameObject player;

    protected SpriteRenderer spriteRenderer;

    public Emotion Emotion { get; set; } = Emotion.Impulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentLife = maxLife;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition.z = -1;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
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

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.GetDamage(damage);
        }
    }
}
