using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace GameFramework.Extension
{
    public interface IRandomChance
    {
        float GetChance { get; }
    }

    public static class Extension
    {
        private static System.Random _random = new System.Random();

        public static void Swap<T>(this IList<T> list, int startIndex, int endIndex)
        {
            if(startIndex > list.Count - 1 || startIndex < 0)
            {
                Debug.LogError("Start Index is out of range");
                return;
            }

            if(endIndex > list.Count - 1 || endIndex < 0)
            {
                Debug.LogError("End Index is out of range");
                return;
            }

            var tempValue = list[startIndex];
            list[startIndex] = list[endIndex];
            list[endIndex] = tempValue;
        }

        public static void Swap(this Array array, int startIndex, int endIndex)
        {
            if (startIndex > array.Length - 1 || array.Length < 0)
            {
                Debug.LogError("Start Index is out of range");
                return;
            }

            if (endIndex > array.Length - 1 || endIndex < 0)
            {
                Debug.LogError("End Index is out of range");
                return;
            }

            var tempValue = array.GetValue(startIndex);
            array.SetValue(array.GetValue(endIndex), startIndex);
            array.SetValue(tempValue, endIndex);
        }

        public static void RandomShuffle<T>(this T[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                var k = _random.Next(n--);
                T temp = array[n];

                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void RandomShuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                var k = _random.Next(n--);
                T temp = list[n];

                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static IEnumerable<T> GetRandomSequence<T>(this IReadOnlyCollection<T> list, int count)
            => list.OrderBy(x => _random.Next()).Distinct().Take(count);

        public static Vector3 GetRandomCirclePoint(this Vector3 point, float range)
        {
            var randomPoint = point + Random.insideUnitSphere * range;
            randomPoint.y = point.y;

            return randomPoint;
        }

        public static Quaternion GetRandomConeQuaternionRotation(this Quaternion quaternion, float coneRotationAngle) 
            => Quaternion.Euler(Random.Range(-coneRotationAngle, coneRotationAngle),
                                Random.Range(-coneRotationAngle, coneRotationAngle),
                                0);

        public static Vector3 Append2(this Transform transformObject, float zOffset = 0.0f)
        {
            var appendPosition = transformObject.position;
            appendPosition.z = zOffset;

            transformObject.position = appendPosition;

            return appendPosition;
        }

        public static Vector3 Append2(this Vector3 position, float zOffset = 0.0f)
               => new Vector3(position.x, position.y, zOffset);

        public static Vector2 FromXZToXY(this Vector3 vector3Value)
               => new Vector2(vector3Value.x, vector3Value.z);

        public static T GetRandomElement<T>(this IList<T> list)
        {
            var lastElementIndex = list.Count;
            return list[Random.Range(0, lastElementIndex)];
        }

        public static T GetRandomElement<T>(this Array array)
        {
            var lastElementIndex = array.Length;
            return (T)array.GetValue(Random.Range(0, lastElementIndex));
        }

        public static T GetRandomElementByChance<T>(this IList<T> list) where T : IRandomChance
        {
            var total = 0.0f;
            var probs = new float[list.Count];

            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = list[i].GetChance;
                total += probs[i];
            }

            var randomPoint = (float)_random.NextDouble() * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                    return list[i];
                randomPoint -= probs[i];
            }

            return list.GetRandomElement();
        }

        public static Vector3 GetCentralPoint(this IReadOnlyCollection<Transform> transforms)
        {
            var finalPoint = Vector3.zero;

            foreach (var currentPoint in transforms)
                finalPoint += currentPoint.position;

            finalPoint /= transforms.Count;

            return finalPoint;
        }

        public static Vector3 GetCentralPoint(this Transform[] transforms)
        {
            var finalPoint = Vector3.zero;

            foreach (var currentPoint in transforms)
                finalPoint += currentPoint.position;

            finalPoint /= transforms.Length;

            return finalPoint;
        }

        public static Vector3 GetNearestPoint(this IEnumerable<Transform> nodes, Vector3 destination)
        {
            var nearestDistance = Mathf.Infinity;
            var finalPoint = Vector3.zero;

            foreach (var currentNode in nodes)
            {
                var distanceToNode = (destination - currentNode.position).sqrMagnitude;
                if (nearestDistance > distanceToNode)
                    continue;

                nearestDistance = distanceToNode;
                finalPoint = currentNode.position;
            }

            return finalPoint;
        }

        public static Vector3 GetCentralPoint(this IReadOnlyCollection<Vector3> positions)
        {
            var finalPoint = Vector3.zero;

            foreach (var currentPoint in positions)
                finalPoint += currentPoint;

            finalPoint /= positions.Count;

            return finalPoint;
        }

        public static Vector2 ToVector2(this Vector3 point, bool invertZAxsis = false)
        {
            return invertZAxsis ? new Vector2(point.x, point.z) : new Vector2(point.x, point.y);
        }

        public static T GetRandomElementByChance<T>(this Array array) where T : IRandomChance
        {
            var total = 0.0f;
            var probs = new float[array.Length];

            for (int i = 0; i < probs.Length; i++)
            {
                var arrayElement = (IRandomChance)array.GetValue(i);
                probs[i] = arrayElement.GetChance;
                total += probs[i];
            }

            var randomPoint = (float)_random.NextDouble() * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                    return (T)array.GetValue(i);
                randomPoint -= probs[i];
            }

            return array.GetRandomElement<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = default;
            component = gameObject.GetComponent<T>();

            if (component != null)
            {
                return component;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return component;
            }
        }

    }
}