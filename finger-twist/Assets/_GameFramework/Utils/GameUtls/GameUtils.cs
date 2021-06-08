using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUtils
{
    private static readonly byte[] key = new byte[8] { 22, 41, 18, 47, 38, 217, 65, 64 };
    private static readonly byte[] iv = new byte[8] { 34, 68, 46, 43, 50, 87, 2, 105 };

    private const char VECTOR_COMPONENTS_SEPARATOR = ',';

    public static string Encrypt(string s)
    {
        var inputbuffer = Encoding.Unicode.GetBytes(s);
        var outputBuffer = DES.Create().CreateEncryptor(key, iv).TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);

        return Convert.ToBase64String(outputBuffer);
    }

    public static string Decrypt(string s)
    {
        var inputbuffer = System.Convert.FromBase64String(s);
        var outputBuffer = DES.Create().CreateDecryptor(key, iv).TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);

        return Encoding.Unicode.GetString(outputBuffer);
    }

    public static bool IsFingerOnUI() => EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    public static bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        var results = new List<RaycastResult>();

        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        var uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        var results = new List<RaycastResult>();

        eventDataCurrentPosition.position = screenPosition;
        uiRaycaster.Raycast(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public static Vector3 CastFromCursor() => CastFromCursor(Mathf.Infinity, 1 << 9);

    public static Vector3 CastFromCursor(float distance) => CastFromCursor(distance, 1 << 9);

    public static Vector3 CastFromCursor(float distance, LayerMask mask)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, distance, mask))
            return hit.point;

        return Vector3.zero;
    }

    public static Vector3 ConstrainToMoveArea(Vector3 p, Collider moveArea)
    {
        var returnPoint = p;
        var min = moveArea.bounds.min;
        var max = moveArea.bounds.max;

        returnPoint.x = Mathf.Clamp(p.x, min.x, max.x);
        returnPoint.z = Mathf.Clamp(p.z, min.z, max.z);

        return returnPoint;
    }

    public static void MoveRect(RectTransform rect, float x, float y)
    {
        var ix = Mathf.FloorToInt(x + 0.5f);
        var iy = Mathf.FloorToInt(y + 0.5f);

        var t = rect.transform;
        t.localPosition += new Vector3(ix, iy);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(rect);
#endif
    }

    public static bool IsDistanceLow(Vector3 point0, Vector3 point1, float distance) => (point0 - point1).sqrMagnitude < distance * distance;

    public static bool IsDistanceHigh(Vector3 point0, Vector3 point1, float distance) => (point0 - point1).sqrMagnitude >= distance * distance;

    public static bool IsQuadRangeLow(Vector3 point0, Vector3 point1, float range)
    {
        if (Mathf.Abs(point0.x - point1.x) < range)
            return Mathf.Abs(point0.z - point1.z) < range;
        return false;
    }

    public static bool IsRangeLow(Vector3 point0, Vector3 point1, float range)
    {
        point0.y = 0.0f;
        point1.y = 0.0f;
        return (point0 - point1).sqrMagnitude < range * range;
    }

    public static bool IsRangeHigh(Vector3 point0, Vector3 point1, float range)
    {
        point0.y = 0.0f;
        point1.y = 0.0f;

        return (point0 - point1).sqrMagnitude >= range * range;
    }

    public static int RoundToInt(float arg) => (int)(arg + 0.5);

    public static float SpringLerp(float strength, float deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;

        var ms = Mathf.RoundToInt(deltaTime * 1000f);
        var cumulative = 0f;

        deltaTime = 0.001f * strength;

        for (int i = 0; i < ms; ++i)
            cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);

        return cumulative;
    }

    /// <summary>
    /// Mathf.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    public static float SpringLerp(float from, float to, float strength, float deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;
        var ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;

        for (int i = 0; i < ms; ++i)
            from = Mathf.Lerp(from, to, deltaTime);
        return from;
    }

    /// <summary>
    /// Vector2.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    public static Vector2 SpringLerp(Vector2 from, Vector2 to, float strength, float deltaTime) => Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));

    /// <summary>
    /// Vector3.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    public static Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime) => Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));

    /// <summary>
    /// Quaternion.Slerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    public static Quaternion SpringLerp(Quaternion from, Quaternion to, float strength, float deltaTime) => Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));

    public static Vector3 Vector3Lerp(Vector3 vector1, Vector3 vector2, float rate)
    {
        if (rate > 1.0)
            return vector2;
        if (rate < 0.0)
            return vector1;
        return new Vector3(vector1.x + (vector2.x - vector1.x) * rate, vector1.y + (vector2.y - vector1.y) * rate, vector1.z + (vector2.z - vector1.z) * rate);
    }

    public static Vector2 Vector2Lerp(Vector2 vector1, Vector2 vector2, float rate)
    {
        if (rate > 1.0)
            return vector2;
        if (rate < 0.0)
            return vector1;
        return new Vector2(vector1.x + (vector2.x - vector1.x) * rate, vector1.y + (vector2.y - vector1.y) * rate);
    }

    public static float Lerp(float from, float to, float rate)
    {
        if (rate < 0.0)
            return from;
        if (rate > 1.0)
            return to;
        return (to - from) * rate + from;
    }

    public static float ClampAngle360(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    public static float ClampAngle180(float angle, float min, float max)
    {
        if (angle < -180)
            angle += 180;
        if (angle > 180)
            angle -= 180;

        return Mathf.Clamp(angle, min, max);
    }

    public static float NoiseFilter(float value, float ratio)
    {
        return (float)(int)(value * ratio) / ratio;
    }

    public static Vector3 NoiseFilter(Vector3 vector, float ratio)
    {
        var zero = Vector3.zero;

        for (int index = 0; index < 3; ++index)
            zero[index] = (float)(int)(vector[index] * ratio) / ratio;

        return zero;
    }

    public static Vector3 CutWithMask(Vector3 vector, Vector3 mask)
    {
        var zero = Vector3.zero;

        for (int index = 0; index < 3; ++index)
            zero[index] = (int)mask[index] != 0 ? 0.0f : vector[index];

        return zero;
    }

    public static Vector3 GetPointOnRay(Ray ray, float height)
    {
        var origin = ray.origin;
        var direction = ray.direction;
        var x = (height - origin.y) * direction.x / direction.y + origin.x;
        var z = (height - origin.y) * direction.z / direction.y + origin.z;

        return new Vector3(x, height, z);
    }

    public static Vector3 GetPointOnRay(Vector3 origin, Vector3 direction, Vector3 point, Vector3 normal)
    {
        if (normal.x > normal.y && normal.x > normal.z)
        {
            var y = (point.x - origin.x) * direction.y / direction.x + origin.y;
            var z = (point.x - origin.x) * direction.z / direction.x + origin.z;

            return new Vector3(point.x, y, z);
        }
        if (normal.y <= normal.x || normal.y <= normal.z)
            return new Vector3((point.z - origin.z) * direction.x / direction.z + origin.x,
                               (point.z - origin.z) * direction.y / direction.z + origin.y,
                               point.z);

        var x = (point.y - origin.y) * direction.x / direction.y + origin.x;
        var z1 = (point.y - origin.y) * direction.z / direction.y + origin.z;

        return new Vector3(x, point.y, z1);
    }

    public static Vector3 GetPointOnRay(Vector3 origin, Vector3 direction, float height)
    {
        var x = (height - origin.y) * direction.x / direction.y + origin.x;
        var z = (height - origin.y) * direction.z / direction.y + origin.z;

        return new Vector3(x, height, z);
    }

    public static Vector3 ParseVector3(string data)
    {
        var strArray = data.Split(',');

        if (strArray.Length == 3)
            return new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));

        return Vector3.zero;
    }

    public static string Vector3ToString(Vector3 vector) => string.Format("{0}{3}{1}{3}{2}", vector.x, vector.y, vector.z, ',');

    public static class JsonUtilityArray
    {
        public static T[] FromJson<T>(string json)
        {
            WrapperArray<T> wrapper = JsonUtility.FromJson<WrapperArray<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            WrapperArray<T> wrapper = new WrapperArray<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            WrapperArray<T> wrapper = new WrapperArray<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class WrapperArray<T>
        {
            public T[] Items;
        }
    }

    public static class JsonUtilityList
    {
        public static List<T> FromJson<T>(string json)
        {
            WrapperList<T> wrapper = JsonUtility.FromJson<WrapperList<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(List<T> list)
        {
            WrapperList<T> wrapper = new WrapperList<T>();
            wrapper.Items = list;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(List<T> list, bool prettyPrint)
        {
            WrapperList<T> wrapper = new WrapperList<T>();
            wrapper.Items = list;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class WrapperList<T>
        {
            public List<T> Items;
        }
    }
}


namespace GameFramework.Protection
{
    [System.Serializable]
    public struct Protected_Int
    {
        /// <summary>
        /// Get encrypted QuestValue (for serialization or something else).
        /// </summary>
        /// <QuestValue>The encrypted QuestValue.</QuestValue>
        public int EncryptedValue
        {
            get
            {
                if (_conv == 0 && value == 0)
                {
                    _conv = XOR_MASK;
                }
                return value;
            }
        }

        private const uint XOR_MASK = 0xaaaaaaaa;

        [SerializeField] private int value;
        private uint _conv;

        public static implicit operator int(Protected_Int v)
        {
            v._conv ^= XOR_MASK;
            var i = v.value;
            v._conv ^= XOR_MASK;
            return i;
        }

        public static implicit operator Protected_Int(int v)
        {
            var p = new Protected_Int();
            p.value = v;
            p._conv ^= XOR_MASK;
            return p;
        }
    }

    [System.Serializable]
    public struct Protected_Float
    {
        /// <summary>
        /// Get encrypted QuestValue (for serialization or something else).
        /// </summary>
        /// <QuestValue>The encrypted QuestValue.</QuestValue>
        public float EncryptedValue
        {
            get
            {
                if (_conv == 0 && value == 0)
                {
                    _conv = XOR_MASK;
                }
                return value;
            }
        }

        private const uint XOR_MASK = 0xaaaaaaaa;

        [SerializeField] private float value;
        private uint _conv;

        public static implicit operator float(Protected_Float v)
        {
            v._conv ^= XOR_MASK;
            var f = v.value;
            v._conv ^= XOR_MASK;
            return f;
        }

        public static implicit operator Protected_Float(float v)
        {
            var p = new Protected_Float();
            p.value = v;
            p._conv ^= XOR_MASK;
            return p;
        }
    }
}

namespace KiizbuyProduction.MathfFast
{
    /// <summary>
    /// Holder of extensions / helpers.
    /// </summary>
    public static class MathfFast
    {
        /// <summary>
        /// PI approximation.
        /// </summary>
        public const float PI = 3.141592654f;
        /// <summary>
        /// PI/2 approximation.
        /// </summary>
        public const float PI_DIV_2 = 3.141592654f * 0.5f;
        /// <summary>
        /// PI*2 approximation.
        /// </summary>
        public const float PI_2 = PI * 2f;
        /// <summary>
        /// Radians to Degrees conversion multiplier.
        /// </summary>
        public const float Rad2Deg = 180f / PI;
        /// <summary>
        /// Degrees to Radians conversion multiplier.
        /// </summary>
        public const float Deg2Rad = PI / 180f;

        public static float Min(float a, float b) => a > b ? b : a;
        public static int Min(int a, int b) => a > b ? b : a;
        public static float Max(float a, float b) => a > b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;

        private const int SinCosIndexMask = ~(-1 << 12);
        private const float SinCosIndexFactor = SinCosCacheSize / PI_2;
        private const int SinCosCacheSize = SinCosIndexMask + 1;
        private const int Atan2Size = 1024;
        private const int Atan2NegSize = -Atan2Size;

        private static readonly float[] _sinCache;
        private static readonly float[] _cosCache;


        private static float[] _atan2CachePPY = new float[Atan2Size + 1];
        private static float[] _atan2CachePPX = new float[Atan2Size + 1];
        private static float[] _atan2CachePNY = new float[Atan2Size + 1];
        private static float[] _atan2CachePNX = new float[Atan2Size + 1];
        private static float[] _atan2CacheNPY = new float[Atan2Size + 1];
        private static float[] _atan2CacheNPX = new float[Atan2Size + 1];
        private static float[] _atan2CacheNNY = new float[Atan2Size + 1];
        private static float[] _atan2CacheNNX = new float[Atan2Size + 1];


        struct FloatInt
        {
            public float Float;
            public int Int;
        }


        struct DoubleInt64
        {
            public double Double;
            public Int64 Int64;
        }

        static MathfFast()
        {
            // Sin/Cos
            _sinCache = new float[SinCosCacheSize];
            _cosCache = new float[SinCosCacheSize];
            int i;

            for (i = 0; i < SinCosCacheSize; i++)
            {
                _sinCache[i] = (float)Math.Sin((i + 0.5f) / SinCosCacheSize * PI_2);
                _cosCache[i] = (float)Math.Cos((i + 0.5f) / SinCosCacheSize * PI_2);
            }

            var factor = SinCosCacheSize / 360f;

            for (i = 0; i < 360; i += 90)
            {
                _sinCache[(int)(i * factor) & SinCosIndexMask] = (float)System.Math.Sin(i * PI / 180f);
                _cosCache[(int)(i * factor) & SinCosIndexMask] = (float)System.Math.Cos(i * PI / 180f);
            }

            // Atan2
            var invAtan2Size = 1f / Atan2Size;

            for (i = 0; i <= Atan2Size; i++)
            {
                _atan2CachePPY[i] = (float)System.Math.Atan(i * invAtan2Size);
                _atan2CachePPX[i] = PI_DIV_2 - _atan2CachePPY[i];
                _atan2CachePNY[i] = -_atan2CachePPY[i];
                _atan2CachePNX[i] = _atan2CachePPY[i] - PI_DIV_2;
                _atan2CacheNPY[i] = PI - _atan2CachePPY[i];
                _atan2CacheNPX[i] = _atan2CachePPY[i] + PI_DIV_2;
                _atan2CacheNNY[i] = _atan2CachePPY[i] - PI;
                _atan2CacheNNX[i] = -PI_DIV_2 - _atan2CachePPY[i];
            }
        }

        /// <summary>
        /// Absolute QuestValue of provided data.
        /// </summary>
        /// <param name="v">Raw data.</param>
        public static float Abs(float v) => v < 0f ? -v : v;

        /// <summary>
        /// /// Absolute QuestValue of provided data.
        /// </summary>
        /// <param name="v">Raw data.</param>
        public static int Abs(int v) => v < 0f ? -v : v;

        /// <summary>
        /// Fast Vector2 normalization with 0.001 threshold error.
        /// </summary>
        /// <returns>Normalized Vector2.</returns>
        public static Vector2 NormalizedFast(this Vector2 v)
        {
            var wrapper = new FloatInt();
            wrapper.Float = v.x * v.x + v.y * v.y;
            wrapper.Int = 0x5f3759df - (wrapper.Int >> 1);
            v.x *= wrapper.Float;
            v.y *= wrapper.Float;
            return v;
        }

        /// <summary>
        /// Fast Vector3 normalization with 0.001 threshold error.
        /// </summary>
        /// <returns>Normalized Vector3.</returns>
        public static Vector3 NormalizedFast(this Vector3 v)
        {
            var wrapper = new FloatInt();
            wrapper.Float = v.x * v.x + v.y * v.y + v.z * v.z;
            wrapper.Int = 0x5f3759df - (wrapper.Int >> 1);
            v.x *= wrapper.Float;
            v.y *= wrapper.Float;
            v.z *= wrapper.Float;
            return v;
        }

        /// <summary>
        /// Fast Sin with 0.0003 threshold error.
        /// </summary>
        /// <param name="v">Angle in radians.</param>
        public static float Sin(float v) => _sinCache[(int)(v * SinCosIndexFactor) & SinCosIndexMask];

        /// <summary>
        /// Fast Cos with 0.0003 threshold error.
        /// </summary>
        /// <param name="v">Angle in radians.</param>
        public static float Cos(float v) => _cosCache[(int)(v * SinCosIndexFactor) & SinCosIndexMask];

        /// <summary>
        /// Fast Atan2 with 0.02 threshold error.
        /// </summary>
        public static float Atan2(float y, float x)
        {
            if (x >= 0)
            {
                if (y >= 0)
                {
                    if (x >= y)
                        return _atan2CachePPY[(int)(Atan2Size * y / x + 0.5)];
                    else
                        return _atan2CachePPX[(int)(Atan2Size * x / y + 0.5)];
                }
                else
                {
                    if (x >= -y)
                        return _atan2CachePNY[(int)(Atan2NegSize * y / x + 0.5)];
                    else
                        return _atan2CachePNX[(int)(Atan2NegSize * x / y + 0.5)];
                }
            }
            else
            {
                if (y >= 0)
                {
                    if (-x >= y)
                        return _atan2CacheNPY[(int)(Atan2NegSize * y / x + 0.5)];
                    else
                        return _atan2CacheNPX[(int)(Atan2NegSize * x / y + 0.5)];
                }
                else
                {
                    if (x <= y)
                        return _atan2CacheNNY[(int)(Atan2Size * y / x + 0.5)];
                    else
                        return _atan2CacheNNX[(int)(Atan2Size * x / y + 0.5)];
                }
            }
        }

        /// <summary>
        /// Clamp data QuestValue to [min;max] range (inclusive).
        /// Not faster than Mathf.Clamp, but performance very close.
        /// </summary>
        /// <param name="data">Data to clamp.</param>
        /// <param name="min">Min range border.</param>
        /// <param name="max">Max range border.</param>
        public static float Clamp(float data, float min, float max)
        {
            if (data < min)
            {
                return min;
            }
            else
            {
                if (data > max)
                {
                    return max;
                }
                return data;
            }
        }

        /// <summary>
        /// Clamp data QuestValue to [min;max] range (inclusive).
        /// Not faster than Mathf.Clamp, but performance very close.
        /// </summary>
        /// <param name="data">Data to clamp.</param>
        /// <param name="min">Min range border.</param>
        /// <param name="max">Max range border.</param>
        public static int Clamp(int data, int min, int max)
        {
            if (data < min)
            {
                return min;
            }
            else
            {
                if (data > max)
                {
                    return max;
                }
                return data;
            }
        }

        /// <summary>
        /// Clamp data QuestValue to [0;1] range (inclusive).
        /// Not faster than Mathf.Clamp01, but performance very close.
        /// </summary>
        /// <param name="data">Data to clamp.</param>
        public static float Clamp01(float data)
        {
            if (data < 0f)
            {
                return 0f;
            }
            else
            {
                if (data > 1f)
                {
                    return 1f;
                }
                return data;
            }
        }

        /// <summary>
        /// Return E raised to specified power.
        /// 2x times faster than System.Math.Exp, but gives 1% error.
        /// </summary>
        /// <param name="power">Target power.</param>
        public static float Exp(float power)
        {
            var c = new DoubleInt64();
            c.Int64 = (long)(1512775 * power + 1072632447) << 32;
            return (float)c.Double;
        }

        /// <summary>
        /// Linear interpolation between "a"-"b" in factor "t". Factor will be automatically clipped to [0;1] range.
        /// </summary>
        /// <param name="a">Interpolate From.</param>
        /// <param name="b">Interpolate To.</param>
        /// <param name="t">Factor of interpolation.</param>
        public static float Lerp(float a, float b, float t)
        {
            if (t <= 0f)
            {
                return a;
            }
            else
            {
                if (t >= 1f)
                {
                    return b;
                }
                return a + (b - a) * t;
            }
        }

        /// <summary>
        /// Linear interpolation between "a"-"b" in factor "t". Factor will not be automatically clipped to [0;1] range.
        /// Not faster than Mathf.LerpUnclamped, but performance very close.
        /// </summary>
        /// <param name="a">Interpolate From.</param>
        /// <param name="b">Interpolate To.</param>
        /// <param name="t">Factor of interpolation.</param>
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Return data raised to specified power.
        /// 4x times faster than System.Math.Pow, 6x times faster than System.Math.Pow, but gives 3% error.
        /// Возвращает число возведенное в опр. степень
        /// Работает в 4-е раза быстрее, чем System.Math.Pow, но выдает ошибки в 3% случаев
        /// </summary>
        /// <param name="data">Data to raise.</param>
        /// <param name="power">Target power.</param>
        public static float PowInaccurate(float data, float power)
        {
            var c = new DoubleInt64();
            c.Double = data;
            c.Int64 = (long)(power * ((c.Int64 >> 32) - 1072632447) + 1072632447) << 32;
            return (float)c.Double;
        }

        /// <summary>
        /// Возвращает данные, повышенные до указанной мощности. Не быстрее, чем Mathf.Pow
        /// </summary>
        /// <param name="data">Data to raise.</param>
        /// <param name="power">Target power.</param>
        public static float Pow(float data, float power) => (float)Math.Pow(data, power);

        /// <summary>
        /// Return largest integer smaller to or equal to data.
        /// Возвращает наибольшее число, меньшее или равное данным
        /// </summary>
        /// <param name="data">Значение для понижения.</param>
        public static float Floor(float data) => data >= 0f ? (int)data : (int)data - 1;

        /// <summary>
        /// Return largest integer smaller to or equal to data.
        /// Возвращает наибольшее целое число, меньшее или равное данным.
        /// </summary>
        /// <param name="data">Значение для понижения.</param>
        public static int FloorToInt(float data) => data >= 0f ? (int)data : (int)data - 1;

        public static Vector3 Vector3Lerp(Vector3 vector1, Vector3 vector2, float rate)
        {
            if (rate > 1.0)
                return vector2;
            if (rate < 0.0)
                return vector1;
            return new Vector3(vector1.x + (vector2.x - vector1.x) * rate, vector1.y
                               + (vector2.y - vector1.y) * rate, vector1.z + (vector2.z - vector1.z) * rate);
        }

        public static Vector2 Vector2Lerp(Vector2 vector1, Vector2 vector2, float rate)
        {
            if (rate > 1.0)
                return vector2;
            if (rate < 0.0)
                return vector1;
            return new Vector2(vector1.x + (vector2.x - vector1.x) * rate, vector1.y + (vector2.y - vector1.y) * rate);
        }
    }


    /// <summary>
    /// Генератор рандомных чисел, основан на XorShift алгоритме.
    /// </summary>
    public sealed class RngFast
    {
        private const double InvMaxIntExOne = 1.0 / (int.MaxValue + 1.0);
        private const double InvIntMax = 1.0 / int.MaxValue;

        private uint _x;
        private uint _y;
        private uint _z;
        private uint _w;

        /// <summary>
        /// Default initialization.
        /// </summary>
        public RngFast() : this(Environment.TickCount) { }

        /// <summary>
        /// Initialization with custom seed.
        /// </summary>
        /// <param name="seed">Seed.</param>
        public RngFast(int seed)
        {
            SetSeed(seed);
        }

        /// <summary>
        /// Include new Seed.
        /// </summary>
        /// <param name="seed">Seed.</param>
        public void SetSeed(int seed)
        {
            _x = (uint)(seed * 1431655781 + seed * 1183186591 + seed * 622729787 + seed * 338294347);
            _y = 842502087;
            _z = 3579807591;
            _w = 273326509;
        }

        /// <summary>
        /// Get current internal state. Use on your risk!
        /// </summary>
        /// <param name="x">Data vector 1.</param>
        /// <param name="y">Data vector 2.</param>
        /// <param name="z">Data vector 3.</param>
        /// <param name="w">Data vector 4.</param>
        public void GetInternalState(out int x, out int y, out int z, out int w)
        {
            x = (int)_x;
            y = (int)_y;
            z = (int)_z;
            w = (int)_w;
        }

        /// <summary>
        /// Set current internal state. Use on your risk!
        /// </summary>
        /// <param name="x">Data vector 1.</param>
        /// <param name="y">Data vector 2.</param>
        /// <param name="z">Data vector 3.</param>
        /// <param name="w">Data vector 4.</param>
        public void SetInternalState(int x, int y, int z, int w)
        {
            _x = (uint)x;
            _y = (uint)y;
            _z = (uint)z;
            _w = (uint)w;
        }

        /// <summary>
        /// /// Get int32 random number from range [0, max).
        /// </summary>
        /// <returns>Random int QuestValue.</returns>
        public int GetInt(int max)
        {
            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (int)((InvMaxIntExOne * (int)(0x7fffffff & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * max);
        }

        /// <summary>
        /// Get int32 random number from range [min, max).
        /// </summary>
        /// <returns>Random int QuestValue.</returns>
        /// <param name="min">Min QuestValue.</param>
        /// <param name="max">Max QuestValue (excluded).</param>
        public int GetInt(int min, int max)
        {
            if (min >= max)
                return min;

            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + (int)((InvMaxIntExOne *
                                 (int)(0x7fffffff & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * (max - min));
        }

        /// <summary>
        /// Get float random number from range [0, 1) or [0, 1] for includeOne=true.
        /// Возвращает рандомное значение из промежутка [min, max] или [min, max] для includeMax=true.
        /// </summary>
        /// <param name="includeOne">Включает 1 значение для "поиска".</param>
        public float GetFloat(bool includeOne = true)
        {
            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (float)((includeOne ? InvIntMax : InvMaxIntExOne) *
                            (int)(0x7fffffff & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))));
        }

        /// <summary>
        /// Get float random number from range [min, max) or [min, max] for includeMax=true.
        /// Возвращает рандомное значение из промежутка [min, max] или [min, max] для includeMax=true.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="min">Мин. значение.</param>
        /// <param name="max">Макс. значение.</param>
        /// <param name="includeMax">Include max QuestValue for searching.</param>
        public float GetFloat(float min, float max, bool includeMax = true)
        {
            if (min >= max)
                return min;

            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + (float)((includeMax ? InvIntMax : InvMaxIntExOne) *
                                  (int)(0x7fffffff & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * (max - min));
        }
    }
}
