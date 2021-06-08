using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public interface IPopupServiceDecorator : IPopupService
    {
        void InjectData(IPopupView popupView, IPopupButton popupButton);
    }
    
    public interface IPopupService
    {
        Transform RootPopupTransform { get; }

        IPopupView ShowPopup(string popupTitleName, string popupDescription, bool showPopup);
        
        IPopupView ShowPopup(string popupTitleName, string popupDescription,
            IDictionary<string, UnityAction> popupButtonsAction);

        void ClosePopup(IPopupView popupView);
    }
}
