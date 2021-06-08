using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public interface ICreatable<out T>
    {
        T Construct();
        void Deconstruct();
    }

    public interface IUpdatableTextData
    {
        void UpdateTextData();
    }
    
    public interface IPopupView : ICreatable<IPopupView>, IUpdatableTextData
    {
        string PopupTitle { get; set; }
        string PopupDescription { get; set; }
        RectTransform PopupRect { get; }
        RectTransform PopupButtonRootRect { get; }
        void CreateButton(IPopupButton popupButton, IDictionary<string, UnityAction> buttons);
        void Show();
        void Hide();
    }

    public interface IPopupButton : ICreatable<IPopupButton>, IUpdatableTextData
    {
        RectTransform ButtonRect { get; }
        string ButtonName { get; set; }
        Button PopupButton { get; }
    }
}