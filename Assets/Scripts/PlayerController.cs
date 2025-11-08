using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Emotions;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject projectilePrefab;
    public float attackCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canAttack = true;
    private Emotion currentEmotion = Emotion.Impulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + movement * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        movement = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Emotion = currentEmotion;

            StartCoroutine(AttackCooldown());
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (System.Enum.TryParse<Emotion>(context.control.name, out var emotion))
        {
            currentEmotion = emotion;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        currentEmotion = Emotion.Impulse;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
