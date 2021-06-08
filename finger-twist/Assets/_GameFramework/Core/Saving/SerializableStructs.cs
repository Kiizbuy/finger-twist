using UnityEngine;

namespace GameFramework.Saving
{
    [System.Serializable]
    public struct SerializableVector3
    {
        private float _x;
        private float _y;
        private float _z;

        public SerializableVector3(Vector3 vector)
        {
            _x = vector.x;
            _y = vector.y;
            _z = vector.z;
        }

        public Vector3 ToUnity() => new Vector3(_x, _y, _z);
    }

    [System.Serializable]
    public struct SerializableVector2
    {
        private float _x;
        private float _y;

        public SerializableVector2(Vector2 vector)
        {
            _x = vector.x;
            _y = vector.y;
        }

        public Vector2 ToUnity() => new Vector2(_x, _y);
    }

    [System.Serializable]
    public struct SerializableQuaternion
    {
        private float _x;
        private float _y;
        private float _z;
        private float _w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            _x = quaternion.x;
            _y = quaternion.y;
            _z = quaternion.z;
            _w = quaternion.w;
        }

        public Quaternion ToUnity() => new Quaternion(_x, _y, _z, _w);
    }
}
