namespace GameFramework.AI.SimpleBehaviorTree
{
    [System.Serializable]
    public abstract class BaseBehaviorNode : IBehaviorNode
    {
        public string NodeName { get; }
        public NodeStatus NodeStatus => _nodeStatus;

        protected BaseBehaviorNode(string nodeName)
        {
            NodeName = nodeName;
        }

        protected NodeStatus _nodeStatus;

        public abstract NodeStatus Evaluate();
    }
}
