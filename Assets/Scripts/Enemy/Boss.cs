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
    public float roamRadius = 3f;
    public float attackTime = 2f;
    public float shieldTime = 20f;
    public int shieldCount = 5;
    public float shieldRadius = 1.5f;
    public GameObject bossObjDeath;

    private ArrayList projectiles;
    private Vector2 neuronCenter;
    private Phase currentPhase = Phase.Phase1;
    private readonly Dictionary<Phase, int> phaseThresholds = new Dictionary<Phase, int>()
    {
        { Phase.Phase1, 120 },
        { Phase.Phase2, 70 },
        { Phase.Phase3, 0 }
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        projectiles = new ArrayList(shieldCount);
        neuronCenter = transform.parent.position;

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
        targetPosition = transform.position;

        while (true)
        {
            yield return new WaitForSeconds(roamTime);

            targetPosition = neuronCenter + Random.insideUnitCircle * roamRadius;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackTime);

            EnemyProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                .GetComponent<EnemyProjectile>();
            projectile.Emotion = Emotion;
            projectile.Direction = ((Vector2)player.transform.position -
                (Vector2)projectile.transform.position).normalized;
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
        GameManager.Instance.LaunchEnableObjectForSecondsCoroutine(bossObjDeath, 7f);
        MapManager.Instance.StartCoroutine(MapManager.Instance.Wait(7f));
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(neuronCenter, Vector3.forward, roamRadius);
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, shieldRadius);
    }
}
