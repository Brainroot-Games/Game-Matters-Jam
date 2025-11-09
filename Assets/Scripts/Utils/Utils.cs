using UnityEngine;

public static class Utils
{
    public static Vector2 bounds = new Vector2(6f, 4.5f);
    public static float boundsRadius = 4f;

    public static Vector3 ClampToBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -bounds.x, bounds.x);
        position.y = Mathf.Clamp(position.y, -bounds.y, bounds.y);
        return position;
    }

    public static bool IsOutsideBounds(Vector3 position)
    {
        return position.x < -bounds.x || position.x > bounds.x ||
               position.y < -bounds.y || position.y > bounds.y;
    }
}
