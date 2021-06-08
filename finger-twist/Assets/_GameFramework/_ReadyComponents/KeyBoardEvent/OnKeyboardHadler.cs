using GameFramework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKeyboardHadler : MonoBehaviour
{
    public event Action<EventParameter> OnKeyDown;

    [EventName(nameof(OnKeyDown))]
    public EventToMethodSubscribeСontainer OnKeyDownSubscriber = new EventToMethodSubscribeСontainer();

    public KeyCode keyCode = KeyCode.S;

    private void Start()
    {
        EventSubscriber.Subscribe(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
            OnKeyDown?.Invoke(null);
    }
}
