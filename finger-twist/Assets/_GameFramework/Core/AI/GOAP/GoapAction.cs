using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public enum ActionState
    {
        Idle,
        Running,
        Error
    }

    public interface IGoapAction
    {
        IReadOnlyDictionary<string, int> Preconditions { get; }
        IReadOnlyDictionary<string, int> Effects { get; }
        string ActionName { get; }
        float Cost { get; }
        float Duration { get; }
        bool IsAchievable();
        bool IsAchievableGiven(Dictionary<string, int> conditions);
        void StartAction();
        void StopAction();
        bool CanStartAction();
        bool ActionRunning();
        bool ActionHasComplete();
        bool PrePerform();
        bool PostPerform();
    }

    public abstract class GoapAction : MonoBehaviour, IGoapAction
    {
        [SerializeField] private string _actionName = "Action";
        [SerializeField] private float _actionCost = 1f;
        [SerializeField] private float _actionDuration = 1f;
        [SerializeField] private WorldState[] _preConditionsSettings;
        [SerializeField] private WorldState[] _effectsSettings;

        protected ActionState _actionState;

        private Dictionary<string, int> _preconditions = new Dictionary<string, int>();
        private Dictionary<string, int> _effects = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Preconditions => _preconditions;
        public IReadOnlyDictionary<string, int> Effects => _effects;
        public string ActionName => _actionName;
        public float Cost => _actionCost;
        public float Duration => _actionDuration;


        public void Awake()
        {
            InitActions();
        }

        protected void InitActions()
        {
            foreach (var w in _preConditionsSettings)
            {
                _preconditions.Add(w.key, w.value);
            }

            foreach (var w in _effectsSettings)
            {
                _effects.Add(w.key, w.value);
            }
        }

        public virtual bool IsAchievable()
        {
            return true;
        }


        public bool IsAchievableGiven(Dictionary<string, int> conditions)
        {
            foreach (var p in _preconditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }

            return true;
        }

        public abstract void StartAction();
        public abstract void ScheduleAction();
        public abstract void StopAction();
        public abstract bool CanStartAction();
        public abstract bool ActionRunning();
        public abstract bool ActionHasComplete();
        public abstract bool PrePerform();
        public abstract bool PostPerform();
    }
}
