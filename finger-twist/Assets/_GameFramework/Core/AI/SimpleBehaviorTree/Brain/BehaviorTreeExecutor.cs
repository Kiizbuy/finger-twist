using UnityEngine;

namespace GameFramework.AI.SimpleBehaviorTree
{
    public interface IBehaviorTreeExecutor
    {
        void Execute(IBehaviorNode rootNode);
    }

    public class BehaviorTreeExecutor : MonoBehaviour, IBehaviorTreeExecutor
    {
        public void Execute(IBehaviorNode rootNode)
        {
            rootNode.Evaluate();
        }
    }
}
