using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public class PopupViewMonoBehaviour : MonoBehaviour, IPopupView
    {
        [SerializeField] private Text _popupTitleLabel;
        [SerializeField] private Text _popupDescriptionLabel;
        [SerializeField] private RectTransform _popupRect;
        [SerializeField] private RectTransform _popupButtonRootRect;

        private readonly List<IPopupButton> _popupButtons = new List<IPopupButton>();
        
        public IPopupView Construct()
        {
            var popup = Instantiate(gameObject).GetComponent<IPopupView>();
            return popup;
        }

        //TODO cancer remove
        public void Deconstruct()
        {
            Destroy(gameObject);
        }

        public string PopupTitle { get; set; }
        public string PopupDescription { get; set; }
        public RectTransform PopupRect => _popupRect;
        public RectTransform PopupButtonRootRect => _popupButtonRootRect;
        
        public void CreateButton(IPopupButton popupButton, IDictionary<string, UnityAction> buttons)
        {
            foreach (var oldButtons in _popupButtons)
                oldButtons.Deconstruct();
            _popupButtons.Clear();

            foreach (var buttonAction in buttons)
            {
                var button = popupButton.Construct();
                button.ButtonRect.SetParent(PopupButtonRootRect);
                button.ButtonName = buttonAction.Key;
                button.PopupButton.onClick.AddListener(buttonAction.Value);
                button.UpdateTextData(); 
                _popupButtons.Add(button);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateTextData()
        {
            _popupTitleLabel.text = PopupTitle;
            _popupDescriptionLabel.text = PopupDescription;
        }
    }
}
