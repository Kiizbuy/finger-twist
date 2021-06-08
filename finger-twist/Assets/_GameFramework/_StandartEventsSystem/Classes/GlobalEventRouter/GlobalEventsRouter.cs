using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Events
{
    public class GlobalEventsRouter : MonoBehaviour
    {
        private Dictionary<string, Action<EventParameter>> _eventDictionary;

        private void Awake()
            => Init();

        private void Init()
            => _eventDictionary = new Dictionary<string, Action<EventParameter>>();

        public void StartListeningGlobalEvent(string eventName, Action<EventParameter> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                _eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary.Add(eventName, thisEvent);
            }

            Debug.Log($"GLOBAL EVENT:{eventName} is listening");
        }

        public void StopListeningGlobalEvent(string eventName, Action<EventParameter> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent -= listener;
                _eventDictionary[eventName] = thisEvent;
            }
        }

        public void RaiseGlobalEvent(string eventName, EventParameter param)
        {
            Debug.Log($"RAISE GLOBAL EVENT: {eventName}");

            if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
                thisEvent.Invoke(param);
        }
    }
}
