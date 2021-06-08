using System;
using GameFramework.Events;
using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.Components
{
    public class Physics3DTriggerCallbackInvoker : MonoBehaviour
    {
        public event Action<EventParameter> OnTriggerEntered;
        public event Action<EventParameter> OnTriggerStayed;
        public event Action<EventParameter> OnTriggerGone;

        [InfoBox("Works only with 'TaggableObject' Component")]
        [EventName(nameof(OnTriggerEntered))]
        public EventToMethodSubscribeСontainer OnTriggerEnterSubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnTriggerStayed))]
        public EventToMethodSubscribeСontainer OnTriggerStaySubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnTriggerGone))]
        public EventToMethodSubscribeСontainer OnTriggerExitSubscriber = new EventToMethodSubscribeСontainer();

        [Space(20f)]
        public bool UseOnTriggerEnter = true;
        public bool UseOnTriggerStay = false;
        public bool UseOnTriggerExit = false;

        [SerializeField, Tag] private string _objectTag;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!UseOnTriggerEnter)
                return;

            var taggableObject = other.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerEntered?.Invoke(new EventParameter_Collider(other));
        }

        private void OnTriggerStay(Collider other)
        {
            if (!UseOnTriggerStay)
                return;

            var taggableObject = other.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerStayed?.Invoke(new EventParameter_Collider(other));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!UseOnTriggerExit)
                return;

            var taggableObject = other.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerGone?.Invoke(new EventParameter_Collider(other));
        }
    }
}

