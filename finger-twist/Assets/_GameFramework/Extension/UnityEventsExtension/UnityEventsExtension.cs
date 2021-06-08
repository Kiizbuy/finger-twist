using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UnityEngine.EventsExtension
{
    [Serializable] public class UnityEventInt : UnityEvent<int> { }
    [Serializable] public class UnityEventFloat : UnityEvent<float> { }
    [Serializable] public class UnityEventString : UnityEvent<string> { }
    [Serializable] public class UnityEventBool : UnityEvent<bool> { }
    [Serializable] public class UnityEventTransform : UnityEvent<Transform> { }
    [Serializable] public class UnityEventGameObject : UnityEvent<GameObject> { }
}
