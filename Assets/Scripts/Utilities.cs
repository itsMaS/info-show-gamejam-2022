using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static T PickRandom<T>(this IEnumerable<T> collection)
    {
        var array = collection.ToArray();
        return array[Random.Range(0, array.Length)];
    }
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    public static bool TryGetComponentInParent<T>(this Component parentComponent, out T component)
    {
        component = parentComponent.GetComponentInParent<T>();
        return component != null;
    }
    public static bool TryGetComponentInChildren<T>(this Component parentComponent, out T component)
    {
        component = parentComponent.GetComponentInChildren<T>();
        return component != null;
    }
    public static void SetOpacity(this SpriteRenderer sr, float opacity)
    {
        Color color = sr.color;
        color.a = opacity;

        sr.color = color;
    }

    public static float PickRandom(this Vector2 range)
    {
        return Mathf.Lerp(range.x, range.y, Random.value);
    }
    public static float Evaluate(this Vector2 range, float t)
    {
        return Mathf.Lerp(range.x, range.y, t);
    }
}
