using System;

namespace GameFramework.Events
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EventNameAttribute : Attribute
    {
        public readonly string EventName;

        public EventNameAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class MethodNameAttribute : Attribute
    {
        public readonly string MethodName;

        public MethodNameAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}

