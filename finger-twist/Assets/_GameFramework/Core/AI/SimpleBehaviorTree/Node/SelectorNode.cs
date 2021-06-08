using System.Collections.Generic;

namespace GameFramework.AI.SimpleBehaviorTree
{
    public class Selector : BaseBehaviorNode
    {
        protected readonly List<BaseBehaviorNode> _nodeLeaves = new List<BaseBehaviorNode>();

        public Selector(string nodeName, IEnumerable<BaseBehaviorNode> nodes) : base(nodeName)
        {
            _nodeLeaves.AddRange(nodes);
        }

        public override NodeStatus Evaluate()
        {
            foreach (var node in _nodeLeaves)
            {
                switch (node.Evaluate())
                {
                    case NodeStatus.Failed:
                        continue;
                    case NodeStatus.Success:
                        _nodeStatus = NodeStatus.Success;
                        return _nodeStatus;
                    case NodeStatus.Running:
                        _nodeStatus = NodeStatus.Running;
                        return _nodeStatus;
                    default:
                        continue;
                }
            }

            _nodeStatus = NodeStatus.Failed;
            return _nodeStatus;
        }
    }
}
