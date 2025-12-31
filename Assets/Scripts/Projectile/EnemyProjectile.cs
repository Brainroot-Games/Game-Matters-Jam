using UnityEngine;

public class EnemyProjectile : Projectile
{
    public float angularSpeed = 100f;
    private Transform rotationCenter = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        targetTag = TagHandle.GetExistingTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationCenter != null)
        {
            transform.RotateAround(rotationCenter.position, Vector3.forward, angularSpeed * Time.deltaTime);
        }
    }

    public void Rotate(Transform center)
    {
        rotationCenter = center;
    }

    protected override void OnTarget(GameObject target)
    {
        Player player = target.GetComponent<Player>();
        player.GetDamage(damage);
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2DBounds(Collider2D other)
    {
        if (rotationCenter == null)
        {
            Destroy(gameObject);
        }
    }
}
