using UnityEngine;

public static class Utils
{
    public static float GetSpeed(Vector2 from, Vector2 to, float time) =>
        Vector2.Distance(from, to) / time;
}
