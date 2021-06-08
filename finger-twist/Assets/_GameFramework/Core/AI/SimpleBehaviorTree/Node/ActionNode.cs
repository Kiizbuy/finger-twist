using System;

namespace GameFramework.AI.SimpleBehaviorTree
{
    public class ActionNode : BaseBehaviorNode
    {
        private readonly Func<NodeStatus> _nodeAction;

        public ActionNode(string nodeName, Func<NodeStatus> nodeAction) : base(nodeName)
        {
            _nodeAction = nodeAction;
        }

        public override NodeStatus Evaluate()
        {
            switch (_nodeAction())
            {
                case NodeStatus.Success:
                    _nodeStatus = NodeStatus.Success;
                    return _nodeStatus;
                case NodeStatus.Failed:
                    _nodeStatus = NodeStatus.Failed;
                    return _nodeStatus;
                case NodeStatus.Running:
                    _nodeStatus = NodeStatus.Running;
                    return _nodeStatus;
                default:
                    _nodeStatus = NodeStatus.Failed;
                    return _nodeStatus;
            }
        }
    }
}
