using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float moveTime = 1f;

    private Vector3 target;
    private Vector3 offset;
    private float speed = 0f;

    public Vector3 Target {
        get => target;
        set {
            target = value + offset;
            speed = Vector2.Distance(transform.position, target) / moveTime;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = transform.position;
        offset = transform.position;
    }

    private void LateUpdate()
    {
        if (transform.position != Target)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                Target,
                speed * Time.deltaTime);
        }
    }
}
