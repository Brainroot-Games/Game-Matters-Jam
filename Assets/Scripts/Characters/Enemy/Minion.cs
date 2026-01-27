using static Emotions;

public class Minion : Enemy
{
    protected override int SameEmotionDamage(int damage)
    {
        return currentLife;
    }

    protected override void SetSprite()
    {
        spriteRenderer.sprite = SpriteManager.Instance.minionSprites[GetEmotionIndex(Emotion)];
    }

    protected override void Move()
    {
        if (player != null)
        {
            targetPosition = player.transform.position;
        }

        base.Move();
    }

    protected override void OnPlayerCollisionEnter(Player player)
    {
        base.OnPlayerCollisionEnter(player);
        Die();
    }
}
