using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

public class PlayerProjectile : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        targetTag = TagHandle.GetExistingTag("Enemy");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Utils.IsOutsideBounds(rb.position))
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTarget(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy.Emotion.Equals(Emotion) || Emotion.Equals(Emotion.Impulse))
        {
            enemy.GetDamage(damage, Emotion);
            Destroy(gameObject);
        }
    }
}
