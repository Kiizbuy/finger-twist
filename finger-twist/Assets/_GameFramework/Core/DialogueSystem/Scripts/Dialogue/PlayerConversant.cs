using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameFramework.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string _playerName;

        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;
        private AIConversant _currentConversant;
        private bool _isChoosing;

        public event Action OnConversationUpdated;

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            _currentConversant = newConversant;
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public void Quit()
        {
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            _isChoosing = false;
            _currentConversant = null;
            OnConversationUpdated();
        }

        public bool IsActive() => _currentDialogue != null;

        public bool IsChoosing() => _isChoosing;

        public string GetText() => _currentNode == null ? string.Empty : _currentNode.GetText();

        public string GetCurrentConversantName() => _isChoosing ? _playerName : _currentConversant.GetName();

        public IEnumerable<DialogueNode> GetChoices() => FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode));

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            _isChoosing = false;
            TriggerEnterAction();
            Next();
        }

        public void Next()
        {
            var numPlayerResponses = FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                _isChoosing = true;
                TriggerExitAction();
                OnConversationUpdated();
                return;
            }

            var children = FilterOnCondition(_currentDialogue.GetAIChildren(_currentNode)).ToArray();
            var randomIndex = Random.Range(0, children.Count());
            _currentNode = children[randomIndex];
            TriggerExitAction();
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public bool HasNext() => FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).Any();

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode) => inputNode.Where(node => node.CheckCondition(GetEvaluators()));

        private IEnumerable<IPredicateEvaluator> GetEvaluators() => GetComponents<IPredicateEvaluator>();

        private void TriggerEnterAction()
        {
            if (_currentNode != null)
            {
                TriggerAction(_currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (_currentNode != null)
            {
                TriggerAction(_currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == string.Empty)
                return;

            foreach (var trigger in _currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
