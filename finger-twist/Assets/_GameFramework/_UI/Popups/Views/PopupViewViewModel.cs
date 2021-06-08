using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityWeld.Binding;

namespace GameFramework.UI
{
    [Binding]
    public class PopupViewViewModel : BaseViewModelMonobehavior, IPopupView
    {
        [Binding]
        public class PopupButtonViewModel : BaseViewModel
        {
            public event UnityAction OnClick;
            
            [Binding]
            public string ButtonTitle { get; set; }
            [Binding]
            public void Clicked() => OnClick?.Invoke();
        }
        
        [SerializeField] private RectTransform _popupRect;
        [SerializeField] private RectTransform _popupButtonRootRect;

        [Binding]
        public ObservableList<PopupButtonViewModel> Buttons { get; set; } = new ObservableList<PopupButtonViewModel>();
                
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

        [Binding]
        public string PopupTitle { get; set; }
        [Binding]
        public string PopupDescription { get; set; }

        public RectTransform PopupRect => _popupRect;
        public RectTransform PopupButtonRootRect => _popupButtonRootRect;
        
        public void CreateButton(IPopupButton popupButton, IDictionary<string, UnityAction> buttons)
        {
            Buttons.Clear();

            foreach (var buttonActions in buttons)
            {
                var popupButtonViewModel = new PopupButtonViewModel()
                {
                    ButtonTitle = buttonActions.Key
                };
                popupButtonViewModel.OnClick += buttonActions.Value;
                Buttons.Add(popupButtonViewModel);
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
            OnPropertyChanged(nameof(PopupTitle));
            OnPropertyChanged(nameof(PopupDescription));
        }
    }
}
