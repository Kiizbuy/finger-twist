using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class ActionNode
    {
        public readonly Dictionary<string, int> State;
        public readonly ActionNode Parent;
        public readonly GoapAction Action;
        public readonly float Cost;

        public ActionNode(ActionNode parent, float cost, IDictionary<string, int> allStates, GoapAction action)
        {
            Parent = parent;
            Cost = cost;
            State = new Dictionary<string, int>(allStates);
            Action = action;
        }
    }

    public class GoapPlanner
    {
        public Queue<GoapAction> GetPlan(List<GoapAction> actions, Dictionary<string, int> goal, WorldStates states)
        {
            var usableActions = actions.Where(a => a.IsAchievable()).ToList();

            var leaves = new List<ActionNode>();
            var start = new ActionNode(null, 0, GWorld.Instance.GetWorld().GetStates(), null);
            var success = BuildGraph(start, leaves, usableActions, goal);
            var cheapest = GetCheapestNode(leaves);
            var result = new List<GoapAction>();

            if (!success)
            {
                Debug.Log("NO PLAN");
                return null;
            }

            var n = cheapest;
            while (n != null)
            {
                if (n.Action != null)
                {
                    result.Insert(0, n.Action);
                }

                n = n.Parent;
            }

            var queue = new Queue<GoapAction>();
            foreach (var a in result)
                queue.Enqueue(a);


#if UNITY_EDITOR
            Debug.Log("The Plan is: ");
            foreach (var a in queue)
                Debug.Log("Q: " + a.ActionName);
#endif

            return queue;
        }

        private ActionNode GetCheapestNode(IEnumerable<ActionNode> leaves)
        {
            ActionNode cheapest = null;

            foreach (var leaf in leaves)
            {
                if (cheapest == null)
                {
                    cheapest = leaf;
                }
                else
                {
                    if (leaf.Cost < cheapest.Cost)
                        cheapest = leaf;
                }
            }

            return cheapest;
        }

        private bool BuildGraph(ActionNode parent, ICollection<ActionNode> leaves, IReadOnlyCollection<GoapAction> usableActions,
            Dictionary<string, int> goal)
        {
            var foundPath = false;
            var currentState = new Dictionary<string, int>(parent.State);

            foreach (var action in usableActions)
            {
                if (!action.IsAchievableGiven(parent.State))
                    continue;

                foreach (var effect in action.Effects)
                {
                    if (!currentState.ContainsKey(effect.Key))
                        currentState.Add(effect.Key, effect.Value);
                }

                var node = new ActionNode(parent, parent.Cost + action.Cost, currentState, action);

                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    var subset = ActionSubset(usableActions, action);
                    var found = BuildGraph(node, leaves, subset, goal);

                    if (found)
                        foundPath = true;
                }
            }

            return foundPath;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, IReadOnlyDictionary<string, int> state)
        {
            return goal.All(g => state.ContainsKey(g.Key));
        }

        private List<GoapAction> ActionSubset(IEnumerable<GoapAction> actions, GoapAction removableActionPredicate)
        {
            return actions.Where(a => !a.Equals(removableActionPredicate)).ToList();
        }
    }
}
