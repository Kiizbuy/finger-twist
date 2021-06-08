using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Events
{
    public class EventParameter
    {
        private readonly Dictionary<string, object> customDataParametersDictionary = new Dictionary<string, object>();

        public virtual GameObject GetGameObject() => null;
        public virtual Transform GetTransform() => null;
        public virtual Vector3 GetPosition() => Vector3.zero;
        public virtual Quaternion GetRotation() => Quaternion.identity;
        public virtual bool GetBool() => false;
        public virtual int GetInt() => 0;
        public virtual float GetFloat() => 0f;
        public virtual string GetString() => string.Empty;

        public bool TryAddCustomDataParameter<T>(string dataName, T dataValue, bool overwriteData = false)
        {
            if (customDataParametersDictionary.ContainsKey(dataName))
            {
                if (overwriteData)
                {
                    customDataParametersDictionary[dataName] = dataValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                customDataParametersDictionary.Add(dataName, dataValue);
                return true;
            }
        }
        public bool TryGetCustomDataParameterValue<T>(string dataName, out T outCustomData)
        {
            if (customDataParametersDictionary.TryGetValue(dataName, out var data))
            {
                if (data is T returnData)
                {
                    outCustomData = returnData;
                    return true;
                }
            }

            outCustomData = default;
            return false;
        }
    }

    public sealed class EventParameter_GameObject : EventParameter
    {
        public readonly GameObject ParamGameObject;

        public override GameObject GetGameObject() => ParamGameObject;
        public override Transform GetTransform() => ParamGameObject.transform;
        public override Quaternion GetRotation() => GetTransform().rotation;
        public override bool GetBool() => ParamGameObject.activeSelf;

        public EventParameter_GameObject(GameObject paramGameObject)
        {
            ParamGameObject = paramGameObject;
        }
    }

    public sealed class EventParameter_Transform : EventParameter
    {
        public readonly Transform ParamTransform;

        public override Transform GetTransform() => ParamTransform;
        public override GameObject GetGameObject() => ParamTransform.gameObject;
        public override Vector3 GetPosition() => ParamTransform.position;
        public override Quaternion GetRotation() => ParamTransform.rotation;
        public override bool GetBool() => GetGameObject().activeSelf;

        public EventParameter_Transform(Transform paramTransform)
        {
            ParamTransform = paramTransform;
        }
    }

    public sealed class EventParameter_Vector3 : EventParameter
    {
        public readonly Vector3 ParamVector3;

        public override Vector3 GetPosition() => ParamVector3;

        public EventParameter_Vector3(Vector3 paramVector3)
        {
            ParamVector3 = paramVector3;
        }
    }

    public sealed class EventParameter_Quaternion : EventParameter
    {
        public readonly Quaternion ParamQuaternion;

        public override Quaternion GetRotation() => ParamQuaternion;

        public EventParameter_Quaternion(Quaternion paramQuaternion)
        {
            ParamQuaternion = paramQuaternion;
        }
    }

    public sealed class EventParameter_Int : EventParameter
    {
        public readonly int ParamInt;

        public override int GetInt() => ParamInt;

        public EventParameter_Int(int paramInt)
        {
            ParamInt = paramInt;
        }
    }

    public sealed class EventParameter_Float : EventParameter
    {
        public readonly float ParamFloat;

        public override float GetFloat() => ParamFloat;

        public EventParameter_Float(float paramFloat)
        {
            ParamFloat = paramFloat;
        }
    }

    public sealed class EventParameter_Bool : EventParameter
    {
        public readonly bool ParamBool;

        public override bool GetBool() => ParamBool;

        public EventParameter_Bool(bool paramBool)
        {
            ParamBool = paramBool;
        }
    }

    public sealed class EventParameter_String : EventParameter
    {
        public readonly string ParamString;

        public override string GetString() => ParamString;

        public EventParameter_String(string paramString)
        {
            ParamString = paramString;
        }
    }

    public sealed class EventParameter_Collision : EventParameter
    {
        public readonly Collision ParamCollision;

        public override Vector3 GetPosition() => ParamCollision.contacts[0].point;
        public override Transform GetTransform() => GetGameObject().transform;
        public override GameObject GetGameObject() => ParamCollision.gameObject;
        public override bool GetBool() => GetGameObject().activeSelf;

        public EventParameter_Collision(Collision paramCollision)
        {
            ParamCollision = paramCollision;
        }
    }

    public sealed class EventParameter_Collision2D : EventParameter
    {
        public readonly Collision2D ParamCollision2D;

        public override Vector3 GetPosition() => ParamCollision2D.contacts[0].point;
        public override Transform GetTransform() => GetGameObject().transform;
        public override GameObject GetGameObject() => ParamCollision2D.gameObject;
        public override bool GetBool() => GetGameObject().activeSelf;

        public EventParameter_Collision2D(Collision2D paramCollision2D)
        {
            ParamCollision2D = paramCollision2D;
        }
    }

    public sealed class EventParameter_Collider : EventParameter
    {
        public readonly Collider ParamCollider;

        public override Vector3 GetPosition() => ParamCollider.transform.position;
        public override Transform GetTransform() => GetGameObject().transform;
        public override GameObject GetGameObject() => ParamCollider.gameObject;
        public override bool GetBool() => GetGameObject().activeSelf;

        public EventParameter_Collider(Collider paramCollider)
        {
            ParamCollider = paramCollider;
        }
    }

    public sealed class EventParameter_Collider2D : EventParameter
    {
        public readonly Collider2D ParamCollider2D;

        public override Vector3 GetPosition() => ParamCollider2D.transform.position;
        public override Transform GetTransform() => GetGameObject().transform;
        public override GameObject GetGameObject() => ParamCollider2D.gameObject;
        public override bool GetBool() => GetGameObject().activeSelf;

        public EventParameter_Collider2D(Collider2D paramCollider2D)
        {
            ParamCollider2D = paramCollider2D;
        }
    }
}

