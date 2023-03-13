using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomUtils
{
    public static class Utils
    {
        public static Vector3 FlatDirection(Vector3 to, Vector3 from)
        {
            to.y = 0f;
            from.y = 0f;
            return to - from;
        }
        
        public static float NormalizedDotProduct(Vector3 a, Vector3 b)
        {
            var dot = Vector3.Dot(a, b);
            return (dot + 1) / 2f;
        }

        public static Vector3 NormalizedFlatDirection(Vector3 to, Vector3 from) => FlatDirection(to, from).normalized;

        public static IEnumerator DieSequence(float animationSpeed, float bulletTime, Action callback = null)
        {
            Time.timeScale = 0.2f;
            yield return new WaitForSecondsRealtime(bulletTime);
            while (Time.timeScale < 0.95f)
            {
                Time.timeScale += Time.deltaTime * animationSpeed;
                yield return null;
            }

            Time.timeScale = 1f;
            yield return new WaitForSecondsRealtime(bulletTime);
            callback?.Invoke();
        }

        public static string ProgressFormat(Vector2Int progress) => $"{progress.x + 1} - {progress.y + 1}";
    }
}