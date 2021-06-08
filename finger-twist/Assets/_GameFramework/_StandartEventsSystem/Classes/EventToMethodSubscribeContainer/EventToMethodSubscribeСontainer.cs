using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Events
{
    [Serializable]
    public class EventToMethodSubscribeСontainer
    {
        [SerializeField] private List<MethodsTemplateData> _methodsTemplateData;

        public IEnumerable<MethodsTemplateData> MethodsTemplateData => _methodsTemplateData;
    }

    [Serializable]
    public class MethodsTemplateData
    {
        [SerializeField] private GameObject _eventObject;
        [SerializeField] private bool _isGlobalEvent;
        [SerializeField] private string _globalEventName;
        [SerializeField] private string _monobehaviourMethodName = "None";
        [SerializeField] private MonoBehaviour _monobehaviourReference;

        public GameObject EventObject => _eventObject;
        public bool IsGlobalEvent => _isGlobalEvent;
        public string GlobalEventName => _globalEventName;
        public string MonobehaviourMethodName => _monobehaviourMethodName;
        public MonoBehaviour MonobehaviourReference => _monobehaviourReference;
    }
}

