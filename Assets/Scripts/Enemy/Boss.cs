using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Emotions;

public class Boss : Enemy
{
    private enum Phase
    {
        Phase1,
        Phase2,
        Phase3
    }

    public float emotionTime = 3f;
    public float roamTime = 5f;
    public float attackTime = 2f;
    public float shieldTime = 20f;
    public int shieldCount = 5;
    public float shieldRadius = 1.5f;
    public GameObject bossObjDeath;

    private Phase currentPhase = Phase.Phase1;
    private Dictionary<Phase, int> phaseThresholds = new Dictionary<Phase, int>()
    {
        { Phase.Phase1, 120 },
        { Phase.Phase2, 70 },
        { Phase.Phase3, 0 }
    };
    private ArrayList projectiles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        projectiles = new ArrayList(shieldCount);

        StartCoroutine(ChangeEmotion());
        StartCoroutine(Roam());
        StartCoroutine(Attack());
        StartCoroutine(Shield());
    }

    public override void GetDamage(int damage, Emotion damageEmotion)
    {
        base.GetDamage(damage, damageEmotion);
        if (currentLife <= phaseThresholds[currentPhase] && currentPhase != Phase.Phase3)
        {
            ChangePhase();
        }
    }

    protected override int SameEmotionDamage(int damage)
    {
        return damage * 2;
    }

    protected override void SetSprite()
    {
        spriteRenderer.sprite = SpriteManager.Instance.bossSprites[(int)currentPhase];
    }

    private void ChangePhase()
    {
        currentPhase++;
        SetSprite();
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

            foreach (EnemyProjectile projectile in projectiles)
            {
                if (projectile != null)
                {
                    projectile.SetEmotion(Emotion);
                }
            }

            yield return new WaitForSeconds(emotionTime);
        }
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            targetPosition = Random.insideUnitCircle * Utils.boundsRadius;
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
            projectile.Direction = ((Vector2)player.transform.position -
                (Vector2)projectile.transform.position).normalized;

            yield return new WaitForSeconds(attackTime);
        }
    }

    private IEnumerator Shield()
    {
        while (true)
        {
            for (int i = 0; i < shieldCount; i ++)
            {
                float angleRad = i * (360f / shieldCount) * Mathf.Deg2Rad;
                Vector2 spawnOffset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * shieldRadius;
                Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;
                EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity, transform)
                    .GetComponent<EnemyProjectile>();
                projectile.Emotion = Emotion;
                projectile.Rotate(transform);
                projectiles.Add(projectile);
            }

            yield return new WaitForSeconds(shieldTime);
        }
    }

    private void OnDestroy() {
        MapManager.Instance.StartCoroutine(EnableForSeconds(bossObjDeath, 7f));
        MapManager.Instance.StartCoroutine(MapManager.Instance.Wait(7f));
    }

    private IEnumerator EnableForSeconds(GameObject obj, float seconds) {
        obj.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Use WaitForSecondsRealtime to wait regardless of time scale
        yield return new WaitForSecondsRealtime(seconds);

        // Restore the original time scale
        Time.timeScale = 1;
        obj.SetActive(false);
    }
}