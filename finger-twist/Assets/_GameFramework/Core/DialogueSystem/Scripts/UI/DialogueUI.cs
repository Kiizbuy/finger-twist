using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Dialogue.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _aiText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _aiResponse;
        [SerializeField] private Transform _choiceRoot;
        [SerializeField] private GameObject _choicePrefab;
        [SerializeField] private Button _quitButton;
        [SerializeField] private TextMeshProUGUI _conversantName;

        private PlayerConversant _playerConversant;

        private void Start()
        {
            _playerConversant = FindObjectOfType<PlayerConversant>(); // TODO - cancel remove
            _playerConversant.OnConversationUpdated += UpdateUi;
            _nextButton.onClick.AddListener(() => _playerConversant.Next());
            _quitButton.onClick.AddListener(() => _playerConversant.Quit());

            UpdateUi();
        }

        private void UpdateUi()
        {
            gameObject.SetActive(_playerConversant.IsActive());

            if (!_playerConversant.IsActive())
                return;

            _conversantName.text = _playerConversant.GetCurrentConversantName();
            _aiResponse.SetActive(!_playerConversant.IsChoosing());
            _choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing());

            if (_playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                _aiText.text = _playerConversant.GetText();
                _nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in _choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (var choice in _playerConversant.GetChoices())
            {
                var choiceInstance = Instantiate(_choicePrefab, _choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                var button = choiceInstance.GetComponentInChildren<Button>();

                textComp.text = choice.GetText();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
