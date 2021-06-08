using System;
using GameFramework.Events;
using UnityEngine;
using NaughtyAttributes;

namespace GameFramework.Components
{
    public class Physics2DCollisionCallbackInvoker : MonoBehaviour
    {
        public event Action<EventParameter> OnCollisionEntered;
        public event Action<EventParameter> OnCollisionStayed;
        public event Action<EventParameter> OnCollisionGone;

        [InfoBox("Works only with 'TaggableObject' Component")]
        [EventName(nameof(OnCollisionEntered))]
        public EventToMethodSubscribeСontainer OnCollisionEnterSubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnCollisionStayed))]
        public EventToMethodSubscribeСontainer OnCollisionStaySubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnCollisionGone))]
        public EventToMethodSubscribeСontainer OnCollisionExitSubscriber = new EventToMethodSubscribeСontainer();

        [Space(20f)]
        public bool UseOnCollisionEnter = true;
        public bool UseOnCollisionStay = false;
        public bool UseOnCollisionExit = false;

        [SerializeField, Tag] private string _objectTag;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!UseOnCollisionEnter)
                return;

            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionEntered?.Invoke(new EventParameter_Collision2D(collision));
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!UseOnCollisionStay)
                return;

            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionEntered?.Invoke(new EventParameter_Collision2D(collision));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!UseOnCollisionExit)
                return;
            
            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionGone?.Invoke(new EventParameter_Collision2D(collision));
        }
    }
}
