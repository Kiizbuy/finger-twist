using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Events
{
    [Serializable]
    public class MethodToEventSubscribeContainer
    {
        [SerializeField] private List<EventsTemplateData> _eventsTemplateData;

        public IEnumerable<EventsTemplateData> EventsTemplateData => _eventsTemplateData;
    }

    [Serializable]
    public class EventsTemplateData
    {
        [SerializeField] private GameObject _eventObject;
        [SerializeField] private bool _isGlobalEvent;
        [SerializeField] private string _globalEventName;
        [SerializeField] private string _monobehaviourEventName = "None";
        [SerializeField] private MonoBehaviour _monobehaviourReference;

        public GameObject EventObject => _eventObject;
        public bool IsGlobalEvent => _isGlobalEvent;
        public string GlobalEventName => _globalEventName;
        public string MonobehaviourEventName => _monobehaviourEventName;
        public MonoBehaviour MonobehaviourReference => _monobehaviourReference;
    }
}
