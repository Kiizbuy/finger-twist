using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public sealed class LabelPresenter : MonoBehaviour, IPresenter<string>
    {
        [SerializeField] private Text _textView;
        [SerializeField] private string _pattern = "{0}";

        public void Present(string value)
        {
            if (_textView == null)
            {
                Debug.LogError("Text view is null", this);
                return;
            }

            _textView.text = string.Format(_pattern, value);
        }
    }
}
