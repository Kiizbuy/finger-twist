using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameFramework.Events
{
    [System.Serializable]
    public class UnityMonobehaviourEventInfo
    {
        public MonoBehaviour MonobehaviourReference;
        public readonly EventInfo MonobehaviourEventInfo;

        public UnityMonobehaviourEventInfo() { }

        public UnityMonobehaviourEventInfo(MonoBehaviour monobehaviourEventReference, EventInfo monobehaviourEventInfo) : this()
        {
            MonobehaviourReference = monobehaviourEventReference;
            MonobehaviourEventInfo = monobehaviourEventInfo;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UnityMonobehaviourEventInfo))
            {
                return false;
            }

            var info = (UnityMonobehaviourEventInfo)obj;
            return EqualityComparer<MonoBehaviour>.Default.Equals(MonobehaviourReference, info.MonobehaviourReference) &&
                   EqualityComparer<EventInfo>.Default.Equals(MonobehaviourEventInfo, info.MonobehaviourEventInfo);
        }

        public override int GetHashCode()
        {
            var hashCode = 1200882815;
            hashCode = hashCode * -1521134295 + EqualityComparer<MonoBehaviour>.Default.GetHashCode(MonobehaviourReference);
            hashCode = hashCode * -1521134295 + EqualityComparer<EventInfo>.Default.GetHashCode(MonobehaviourEventInfo);
            return hashCode;
        }
    }
}
