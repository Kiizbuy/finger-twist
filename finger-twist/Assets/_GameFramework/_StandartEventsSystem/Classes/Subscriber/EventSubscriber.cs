using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace GameFramework.Events
{
    public static class EventSubscriber
    {
        private static GlobalEventsRouter _globalEventsRouter;

        public static void Subscribe(object eventObject)
        {
            ///TODO: Refactor to Zenject Bind
            _globalEventsRouter = Object.FindObjectOfType<GlobalEventsRouter>();

            var eventObjectFields = GetEventsAndMethodsFields(eventObject);

            foreach (var field in eventObjectFields)
            {
                if (field.FieldType == typeof(EventToMethodSubscribeСontainer))
                {
                    if (!(field.GetCustomAttribute<EventNameAttribute>() is EventNameAttribute eventNameAttribute))
                    {
                        Debug.LogError($"Error in {eventObject.GetType().Name} -> {field.Name} doesnt Have an EventNameAttribute ");
                        continue;
                    }

                    var eventToMethodSubscriberField = field.GetValue(eventObject) as EventToMethodSubscribeСontainer;
                    SubscribeEvent(eventObject, eventToMethodSubscriberField, eventNameAttribute);
                }

                if (field.FieldType == typeof(MethodToEventSubscribeContainer))
                {
                    if (!(field.GetCustomAttribute<MethodNameAttribute>() is MethodNameAttribute methodNameAttribute))
                    {
                        Debug.LogError($"Error in {eventObject.GetType().Name} -> {field.Name} doesnt Have an MethodNameAttribute ");
                        continue;
                    }

                    var methodToEventSubscriberField = field.GetValue(eventObject) as MethodToEventSubscribeContainer;
                    SubscribeMethod(eventObject, methodToEventSubscriberField, methodNameAttribute);
                }
            }
        }

        private static IEnumerable<FieldInfo> GetEventsAndMethodsFields(object eventObject)
        {
            return eventObject.GetType()
                              .GetRuntimeFields()
                              .Where(x => x.FieldType == typeof(EventToMethodSubscribeСontainer) || x.FieldType == typeof(MethodToEventSubscribeContainer));
        }

        private static void SubscribeEvent(object eventObject, EventToMethodSubscribeСontainer eventToMethodSubscriberField, EventNameAttribute eventNameAttribute)
        {
            if (eventNameAttribute == null)
            {
                Debug.LogError("Event Name Attribute is not found");
                return;
            }

            var eventInfo = eventObject.GetType().GetEvent(eventNameAttribute.EventName);

            if (eventInfo == null)
            {
                Debug.LogError($"ERROR! Event {eventNameAttribute.EventName} doesn't exsist on a object");
                return;
            }

            foreach (var item in eventToMethodSubscriberField.MethodsTemplateData)
            {
                if (item.EventObject == null)
                    continue;

                if (item.IsGlobalEvent)
                {
                    Action<EventParameter> globalEventAction = null;

                    globalEventAction += eventParameter => _globalEventsRouter.RaiseGlobalEvent(item.GlobalEventName, eventParameter);
                    eventInfo.AddEventHandler(eventObject, globalEventAction);
                }
                else
                {
                    if (item.EventObject == null)
                        continue;

                    if (item.MonobehaviourReference == null)
                    {
                        Debug.LogError($"Monobehaviour reference is null. Can't subscribe it");
                        continue;
                    }

                    var newEventDelegate = (Action<EventParameter>)Delegate.CreateDelegate(
                        type: typeof(Action<EventParameter>),
                        target: item.MonobehaviourReference,
                        method: item.MonobehaviourMethodName);

                    eventInfo.RemoveEventHandler(eventObject, newEventDelegate);
                    eventInfo.AddEventHandler(eventObject, newEventDelegate);
                }
            }
        }

        private static void SubscribeMethod(object eventObject, MethodToEventSubscribeContainer methodToEventSubscriberField, MethodNameAttribute methodNameAttribute)
        {
            if (methodNameAttribute == null)
            {
                Debug.LogError("Method Name Attribute is not found");
                return;
            }

            var methodInfo = eventObject.GetType().GetMethod(methodNameAttribute.MethodName);

            if (methodInfo == null)
            {
                Debug.LogError($"ERROR! Method {methodNameAttribute.MethodName} doesn't exsist on a object");
                return;
            }

            foreach (var item in methodToEventSubscriberField.EventsTemplateData)
            {
                if (item.EventObject == null)
                    continue;

                if (item.IsGlobalEvent)
                {
                    var globalAction = (Action<EventParameter>)Delegate.CreateDelegate(
                        type: typeof(Action<EventParameter>),
                        target: eventObject,
                        method: methodInfo.Name);

                    _globalEventsRouter.StartListeningGlobalEvent(item.GlobalEventName, globalAction);
                }
                else
                {
                    var eventInfo = item.MonobehaviourReference.GetType().GetEvent(item.MonobehaviourEventName);
                    var newEventDelegate = (Action<EventParameter>)Delegate.CreateDelegate(
                        type: eventInfo.EventHandlerType,
                        target: eventObject,
                        method: methodInfo.Name);

                    //eventInfo.RemoveEventHandler(item.MonobehaviourReference, newEventDelegate);
                    eventInfo.AddEventHandler(item.MonobehaviourReference, newEventDelegate);
                }
            }
        }
    }
}