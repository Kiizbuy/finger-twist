using System;
using GameFramework.Events;
using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.Components
{
    public class Physics2DTriggerCallbackInvoker : MonoBehaviour
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


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!UseOnTriggerEnter)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerEntered?.Invoke(new EventParameter_Collider2D(collision));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(!UseOnTriggerStay)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerStayed?.Invoke(new EventParameter_Collider2D(collision));
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!UseOnTriggerExit)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerGone?.Invoke(new EventParameter_Collider2D(collision));
        }
    }
}
