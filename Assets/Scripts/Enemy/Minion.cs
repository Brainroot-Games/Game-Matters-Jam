public class Minion : Enemy
{
    protected override int SameEmotionDamage(int damage)
    {
        return currentLife;
    }

    protected override void SetSprite()
    {
        spriteRenderer.sprite = SpriteManager.Instance.minionSprites[(int)Emotion];
    }

    protected override void Move()
    {
        targetPosition = player.position;
        base.Move();
    }

    protected override void OnPlayerCollisionEnter(Player player)
    {
        base.OnPlayerCollisionEnter(player);
        Destroy(gameObject);
    }
}
