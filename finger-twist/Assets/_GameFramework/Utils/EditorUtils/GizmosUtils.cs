using UnityEngine;

namespace GameFramework.Utils.Gizmos
{
    public static class GizmosUtils
    {
        public static void DrawWireDisk(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            var oldColor = global::UnityEngine.Gizmos.color;
            var oldMatrix = global::UnityEngine.Gizmos.matrix;

            global::UnityEngine.Gizmos.color = color;
            global::UnityEngine.Gizmos.matrix = Matrix4x4.TRS(position, rotation, new Vector3(1, 0.01f, 1));
            global::UnityEngine.Gizmos.DrawWireSphere(Vector3.zero, radius);
            global::UnityEngine.Gizmos.matrix = oldMatrix;
            global::UnityEngine.Gizmos.color = oldColor;
        }

        public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
        {
            var srcAngles = GetAnglesFromDir(position, dir);
            var initialPos = position;
            var posA = initialPos;
            var stepAngles = anglesRange / maxSteps;
            var angle = srcAngles - anglesRange / 2;

            for (var i = 0; i <= maxSteps; i++)
            {
                var rad = Mathf.Deg2Rad * angle;
                var posB = initialPos;
                posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                global::UnityEngine.Gizmos.DrawLine(posA, posB);

                angle += stepAngles;
                posA = posB;
            }

            global::UnityEngine.Gizmos.DrawLine(posA, initialPos);
        }

        private static float GetAnglesFromDir(Vector3 position, Vector3 dir)
        {
            var forwardLimitPos = position + dir;
            var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

            return srcAngles;
        }
    }
}
