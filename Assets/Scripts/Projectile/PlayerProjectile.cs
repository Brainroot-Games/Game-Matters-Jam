using UnityEngine;
using static Emotions;

public class PlayerProjectile : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        targetTag = TagHandle.GetExistingTag("Enemy");

        AddListeners();
    }

    private void AddListeners()
    {
        EventManager.Instance.onPlayerInSynapse.AddListener(OnPlayerInSynapse);
    }

    public void OnPlayerInSynapse(Synapse synapse)
    {
        Destroy(gameObject);
    }

    protected override void OnTargetTriggerEnter(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy.Emotion.Equals(Emotion) || Emotion.Equals(Emotion.Neutral))
        {
            enemy.GetDamage(damage, Emotion);
            Destroy(gameObject);
        }
    }
}
