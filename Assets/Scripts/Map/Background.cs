using UnityEngine;

public class Background : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.7f;

    private Transform cameraTransform;
    private Vector3 prevCameraPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = Camera.main.transform;
        prevCameraPosition = cameraTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cameraMovement = (Vector2)(cameraTransform.position - prevCameraPosition);
        transform.Translate(parallaxFactor * cameraMovement);
        prevCameraPosition = cameraTransform.position;
    }
}
