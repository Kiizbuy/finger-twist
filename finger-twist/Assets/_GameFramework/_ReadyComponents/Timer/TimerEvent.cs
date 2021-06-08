using GameFramework.Events;
using NaughtyAttributes;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameFramework.Components
{
    public class TimerEvent : MonoBehaviour
    {
        public event Action<EventParameter> OnTimerHasReached;
        public event Action<EventParameter> OnTimerHasStarted;
        public event Action<EventParameter> OnTimerHasStopped;

        [EventName(nameof(OnTimerHasReached))]
        public EventToMethodSubscribeСontainer OnTimerHasReachedSubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnTimerHasStarted))]
        public EventToMethodSubscribeСontainer OnTimerHasStartedSubscriber = new EventToMethodSubscribeСontainer();
        [EventName(nameof(OnTimerHasStopped))]
        public EventToMethodSubscribeСontainer OnTimerHasStoppedSubscriber = new EventToMethodSubscribeСontainer();

        [Space(5f)]
        [MethodName(nameof(StartTimer))]
        public MethodToEventSubscribeContainer StartTimerActSubscriber = new MethodToEventSubscribeContainer();
        [MethodName(nameof(StopTimer))]
        public MethodToEventSubscribeContainer StopTimerActSubscriber = new MethodToEventSubscribeContainer();

        public bool UseRandomIntervalValue;

        [HideIf(EConditionOperator.And, nameof(UseRandomIntervalValue))]
        public float IntervalTimer = 5f;
        [ShowIf(EConditionOperator.And, nameof(UseRandomIntervalValue), nameof(UseRandomIntervalValue))]
        public float MinInterval = 5f;
        [ShowIf(EConditionOperator.And, nameof(UseRandomIntervalValue), nameof(UseRandomIntervalValue))]
        public float MaxInterval = 7f;

        [SerializeField]
        private bool _loop = true;
        [SerializeField]
        private bool _canProcessTimer;
        [SerializeField, ReadOnly]
        private float _currentTimer = 0;

        private float RandomIntervalValue => Random.Range(MinInterval, MaxInterval);

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

        private void Update()
        {
            if (!_canProcessTimer)
                return;

            if (_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;

                if (_currentTimer <= 0)
                {
                    if (UseRandomIntervalValue)
                        _currentTimer = RandomIntervalValue;

                    OnTimerHasReached?.Invoke(null);
                    _canProcessTimer = _loop;
                }
            }
        }

        public void StopTimerAct() => StopTimer(null);
        public void StartTimerAct() => StopTimer(null);
        public void SetLoop(bool loopValue) => _loop = loopValue;

        public void StopTimer(EventParameter eventParameter)
        {
            _canProcessTimer = false;

            OnTimerHasStopped?.Invoke(eventParameter);
        }

        public void StartTimer(EventParameter eventParameter)
        {
            _currentTimer = UseRandomIntervalValue ? RandomIntervalValue : IntervalTimer;
            _canProcessTimer = true;

            OnTimerHasStarted?.Invoke(eventParameter);
        }
    }
}