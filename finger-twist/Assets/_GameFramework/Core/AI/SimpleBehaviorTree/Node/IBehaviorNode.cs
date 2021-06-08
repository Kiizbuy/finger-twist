namespace GameFramework.AI.SimpleBehaviorTree
{
    public enum NodeStatus
    {
        Success,
        Running,
        Failed
    }

    public interface IBehaviorNode
    {
        string NodeName { get; }
        NodeStatus NodeStatus { get; }
        NodeStatus Evaluate();
    }
}
