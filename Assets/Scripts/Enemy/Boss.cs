using System.Collections;
using UnityEngine;
using static Emotions;

public class Boss : Enemy
{
    public float emotionTime = 3f;
    public float roamTime = 5f;
    public float attackTime = 2f;
    public float specialAttackTime = 20f;
    public int specialAttackCount = 5;
    public float specialAttackRadius = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ChangeEmotion());
        StartCoroutine(Roam());
        StartCoroutine(Attack());
        StartCoroutine(SpecialAttack());
    }

    protected override int SameEmotionDamage(int damage)
    {
        return damage * 2;
    }

    private IEnumerator ChangeEmotion()
    {
        while (true)
        {
            Emotion newEmotion;
            do
            {
                newEmotion = (Emotion)Random.Range(1, 6);
            } while (newEmotion.Equals(Emotion));
            Emotion = newEmotion;
            SetColor();

            yield return new WaitForSeconds(emotionTime);
        }
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            targetPosition = Random.insideUnitCircle * Utils.boundsRay;
            yield return new WaitForSeconds(roamTime);
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            EnemyProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                .GetComponent<EnemyProjectile>();
            projectile.Emotion = Emotion;
            projectile.Direction = (player.transform.position - projectile.transform.position).normalized;

            yield return new WaitForSeconds(attackTime);
        }
    }

    private IEnumerator SpecialAttack()
    {
        while (true)
        {
            for (int i = 0; i < specialAttackCount; i ++)
            {
                float angleRad = i * (360f / specialAttackCount) * Mathf.Deg2Rad;
                Vector2 spawnOffset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * specialAttackRadius;
                Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;
                EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity, transform)
                    .GetComponent<EnemyProjectile>();
                projectile.Emotion = Emotion;
                projectile.Rotate(transform);
            }

            yield return new WaitForSeconds(specialAttackTime);
        }
    }
}
