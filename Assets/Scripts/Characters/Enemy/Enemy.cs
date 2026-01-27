using System.Collections;
using UnityEngine;
using static Emotions;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    protected const int minLife = 0;

    public int maxLife = 30;
    [Min(0)]
    public int damage = 20;
    [Min(0)]
    public float speed = 2.5f;
    [Min(0)]
    public float stopAt = 0.1f;
    [Min(0)]
    public float moveAnimationTime = 0.2f;
    public GameObject projectilePrefab;

    protected int currentLife;
    protected Vector2 targetPosition;
    protected Player player;
    protected SpriteRenderer spriteRenderer;

    private float sqrStopAt;
    private Rigidbody2D rb;
    private bool canMove = true;
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
        canMove = !player.IsInSynapse;

        AddListeners();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
        else
        {
            isMoving = false;
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

    private void AddListeners()
    {
        EventManager.Instance.onPlayerInSynapse.AddListener(OnPlayerInSynapse);
        EventManager.Instance.onPlayerOutSynapse.AddListener(OnPlayerOutSynapse);
    }

    public void OnPlayerInSynapse(Synapse synapse)
    {
        canMove = false;
    }

    public void OnPlayerOutSynapse(Synapse synapse)
    {
        canMove = true;
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

    private void OnValidate()
    {
        maxLife = Mathf.Max(minLife, maxLife);
    }
}
