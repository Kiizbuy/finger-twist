using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class Goal
    {
        public readonly Dictionary<string, int> Goals;
        public readonly bool Remove;

        public Goal(string s, int i, bool r)
        {
            Goals = new Dictionary<string, int> { { s, i } };
            Remove = r;
        }
    }

    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private List<GoapAction> _actions = new List<GoapAction>();
        [SerializeField] private GoapAction CurrentAction;

        private readonly Dictionary<Goal, int> _goals = new Dictionary<Goal, int>();
        private Queue<GoapAction> _actionsQueue = new Queue<GoapAction>();

        private GoapPlanner _planner;
        private Goal _currentGoal;
        private bool _invoked;

        public void Start()
        {
            var acts = GetComponents<GoapAction>();

            foreach (var a in acts)
                _actions.Add(a);

            _planner = new GoapPlanner();
        }

        public void AddGoal(Goal goal, int goalPriority)
        {
            _goals.Add(goal, goalPriority);
        }

        public void CompleteCurrentAction()
        {
            CurrentAction.StopAction();
            CurrentAction.PostPerform();
        }

        private void LateUpdate()
        {
            if (_actionsQueue.Count <= 0)
            {
                TryGetPlanOfActions();
            }

            TryRemoveCompletedGoal();

            if (CurrentAction != null && CurrentAction.ActionRunning())
            {
                if (!CurrentAction.ActionHasComplete())
                    return;
                if (_invoked)
                    return;

                Invoke(nameof(CompleteCurrentAction), CurrentAction.Duration); // TODO - cancer remove that shit
                _invoked = true;

                return;
            }

            if (_actionsQueue.Count <= 0)
                return;

            CurrentAction = _actionsQueue.Dequeue();

            if (CurrentAction.PrePerform())
            {
                if (CurrentAction.CanStartAction())
                    CurrentAction.StartAction();
            }
            else
            {
                _actionsQueue.Clear();
            }

            CurrentAction.ScheduleAction();
        }

        private void TryGetPlanOfActions()
        {
            var sortedGoals = _goals.OrderByDescending(entry => entry.Value);

            foreach (var sg in sortedGoals)
            {
                _actionsQueue = _planner.GetPlan(_actions, sg.Key.Goals, null);
                if (_actionsQueue == null)
                    continue;

                _currentGoal = sg.Key;
                break;
            }
        }

        private void TryRemoveCompletedGoal()
        {
            if (_actionsQueue == null || _actionsQueue.Count != 0)
                return;

            if (_currentGoal.Remove)
                _goals.Remove(_currentGoal);
            _planner = null;
        }
    }
}
