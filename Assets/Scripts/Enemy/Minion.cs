using System.Collections;
using UnityEngine;

public class Minion : Enemy
{
    public float updateTargetTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateTarget());
    }

    protected override int SameEmotionDamage(int damage)
    {
        return currentLife;
    }

    protected override void SetSprite()
    {
        spriteRenderer.sprite = SpriteManager.Instance.minionSprites[(int)Emotion];
    }

    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            targetPosition = player.transform.position;
            yield return new WaitForSeconds(updateTargetTime);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
