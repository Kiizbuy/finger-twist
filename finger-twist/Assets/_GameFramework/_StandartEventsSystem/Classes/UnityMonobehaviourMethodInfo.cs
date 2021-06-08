using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameFramework.Events
{
    [System.Serializable]
    public class UnityMonobehaviourMethodInfo
    {
        public MonoBehaviour MonobehaviourReference;
        public readonly MethodInfo MonobehaviourMethodInfo;

        public UnityMonobehaviourMethodInfo() { }

        public UnityMonobehaviourMethodInfo(MonoBehaviour monobehaviourReference, MethodInfo monobehaviourMethodInfo) : this()
        {
            MonobehaviourReference = monobehaviourReference;
            MonobehaviourMethodInfo = monobehaviourMethodInfo;
        }

        public override bool Equals(object obj)
        {
            if (obj is UnityMonobehaviourMethodInfo equitableEventInfo)
            {
                return MonobehaviourReference == equitableEventInfo.MonobehaviourReference && MonobehaviourMethodInfo == equitableEventInfo.MonobehaviourMethodInfo;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -69702416;
            hashCode = hashCode * -1521134295 + EqualityComparer<MonoBehaviour>.Default.GetHashCode(MonobehaviourReference);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(MonobehaviourMethodInfo);
            return hashCode;
        }
    }
}

