using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool _isPlayerSpeaking;
        [SerializeField] private string _text;
        [SerializeField] private string _onEnterAction;
        [SerializeField] private string _onExitAction;
        [SerializeField] private Condition _condition;
        [SerializeField] private List<string> _children = new List<string>();
        [SerializeField] private Rect _rect = new Rect(0, 0, 200, 100);

        public Rect GetRect() => _rect;

        public string GetText() => _text;

        public ICollection<string> GetChildren() => _children;
        public bool IsPlayerSpeaking() => _isPlayerSpeaking;
        public string GetOnEnterAction() => _onEnterAction;
        public string GetOnExitAction() => _onExitAction;
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators) => _condition.Check(evaluators);

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            _rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText != _text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                _text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childId);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            _isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
