using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public sealed class PopupService : IPopupService, IDisposable
    {
        private readonly Canvas _popupCanvas;
        private readonly IPopupButton _popupButton;
        private readonly IPopupView _popupView;

        private IPopupView _currentPopupView;

        public PopupService(Canvas canvas, IPopupView popupView, IPopupButton popupButton)
        {
            _popupCanvas = canvas;
            _popupView = popupView;
            _popupButton = popupButton;
        }

        public Transform RootPopupTransform => _popupCanvas.transform;

        public IPopupView ShowPopup(string popupTitleName, string popupDescription, bool showPopup = true)
        {
            if (_currentPopupView == null)
                _currentPopupView = _popupView.Construct();
            
            _currentPopupView.PopupTitle = popupTitleName;
            _currentPopupView.PopupDescription = popupDescription;
            _currentPopupView.UpdateTextData();
            _currentPopupView.PopupRect.SetParent(_popupCanvas.transform);

            if (showPopup)
                _currentPopupView.Show();

            return _currentPopupView;
        }

        public IPopupView ShowPopup(string popupTitleName, string popupDescription,
            IDictionary<string, UnityAction> popupButtonsAction)
        {
            var popup = ShowPopup(popupTitleName, popupDescription);

            
                popup.CreateButton(_popupButton, popupButtonsAction);

            popup.Show();

            return popup;
        }

        public void ClosePopup(IPopupView popupView)
        {
            popupView.Hide();
        }

        public void Dispose()
        {
            _currentPopupView = null;
        }
    }
}