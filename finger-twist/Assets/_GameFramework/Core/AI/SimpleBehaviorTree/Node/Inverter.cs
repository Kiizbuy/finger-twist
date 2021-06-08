namespace GameFramework.AI.SimpleBehaviorTree
{
    public class Inverter : BaseBehaviorNode
    {
        private readonly BaseBehaviorNode _node;

        public Inverter(string nodeName, BaseBehaviorNode node) : base(nodeName)
        {
            _node = node;
        }

        public override NodeStatus Evaluate()
        {
            switch (_node.Evaluate())
            {
                case NodeStatus.Failed:
                    _nodeStatus = NodeStatus.Success;
                    return _nodeStatus;
                case NodeStatus.Success:
                    _nodeStatus = NodeStatus.Failed;
                    return _nodeStatus;
                case NodeStatus.Running:
                    _nodeStatus = NodeStatus.Running;
                    return _nodeStatus;
                default:
                    _nodeStatus = NodeStatus.Success;
                    return _nodeStatus;
            }
        }
    }
}
