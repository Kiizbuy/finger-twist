using GameFramework.Common.ResourcesCore;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;
using UnityWeld.Binding.Internal;

namespace GameFramework.UI.WeldExtenstions
{
    [RequireComponent(typeof(Image))]
    public class ImageSpriteBinding : AbstractMemberBinding
    {
        [SerializeField] private bool _disableIfTransparent = true;
        [SerializeField] private string _viewModelPropertyName;

        private Image _image;
        private PropertyWatcher _viewModelWatcher;
        private string _currentSpritePath;

        private readonly IResourceFactory _resourceFactory = new ResourceFactory();

        public string SpritePath
        {
            set
            {
                if (_currentSpritePath == value)
                    return;

                _currentSpritePath = value;

                if (string.IsNullOrEmpty(_currentSpritePath))
                {
                    if (_disableIfTransparent)
                        gameObject.SetActive(false);
                    _image.sprite = null;
                }
                else
                {
                    _image.sprite = null;
                    _resourceFactory.Create<Sprite>(_currentSpritePath, (sprite) =>
                    {
                        _image.sprite = sprite;

                        if (_disableIfTransparent)
                            gameObject.SetActive(true);
                    });
                }
            }
        }

        public bool DisableIfTransparent
        {
            get => _disableIfTransparent;
            set => _disableIfTransparent = value;
        }

        public string ViewModelPropertyName
        {
            get => _viewModelPropertyName;
            set => _viewModelPropertyName = value;
        }

        public override void Init()
        {
            _image = GetComponent<Image>();
            base.Init();
        }

        public override void Connect()
        {
            var source = MakeViewModelEndPoint(_viewModelPropertyName, null, null);
            var propertySync = new PropertySync(source, new PropertyEndPoint(this, nameof(SpritePath), null, null, "view", this), null, this);

            _viewModelWatcher = source.Watch(() => propertySync.SyncFromSource());

            propertySync.SyncFromSource();
        }

        public override void Disconnect()
        {
            if (_viewModelWatcher == null)
                return;
            _viewModelWatcher.Dispose();
            _viewModelWatcher = null;
        }
    }
}
