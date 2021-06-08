using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public class ScreenMonobehavior : MonoBehaviour, IScreen
    {
        public UnityEvent OnPanelShow;
        public UnityEvent OnPanelHide;

        public bool HideAfterShowAnotherPanel => _hideAfterShowAnotherPanel;

        [SerializeField]
        private bool _hideAfterShowAnotherPanel = true;

        public void Show() => OnPanelShow?.Invoke();
        public void Hide() => OnPanelHide?.Invoke();
    }
}
