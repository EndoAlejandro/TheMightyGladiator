using System.Collections.Generic;
using UnityEngine;

namespace CustomUtils
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
                /*var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;*/
            }
        }
    }
    
    public static class Vector3Extensions
    {
        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
        }

        public static Vector3 Plus(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            float newX = (float)(x != null ? original.x + x : original.x);
            float newY = (float)(y != null ? original.y + y : original.y);
            float newZ = (float)(z != null ? original.z + z : original.z);
            return new Vector3(newX, newY, newZ);
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 With(this Vector2 original, float? x = null, float? y = null)
        {
            return new Vector2(x ?? original.x, y ?? original.y);
        }

        public static float GetRandomPointInRange(this Vector2 original)
        {
            return Random.Range(original.x, original.y);
        }

        public static float GetPointInRange(this Vector2 original, float normalizedValue)
        {
            return Mathf.Lerp(original.x, original.y, normalizedValue);
        }

        public static float GetPointInRange(this Vector2Int original, float normalizedValue)
        {
            return Mathf.Lerp(original.x, original.y, normalizedValue);
        }
    }
}