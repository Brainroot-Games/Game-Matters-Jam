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
            targetPosition = player.position;
            yield return new WaitForSeconds(updateTargetTime);
        }
    }

    protected override void OnCollisionEnter2DPlayer(Player player)
    {
        base.OnCollisionEnter2DPlayer(player);
        Destroy(gameObject);
    }
}
