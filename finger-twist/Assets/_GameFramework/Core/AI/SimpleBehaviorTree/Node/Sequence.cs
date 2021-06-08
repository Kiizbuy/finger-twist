using System.Collections.Generic;

namespace GameFramework.AI.SimpleBehaviorTree
{
    public class Sequence : BaseBehaviorNode
    {
        protected readonly List<BaseBehaviorNode> _nodeLeaves;

        public Sequence(string nodeName, List<BaseBehaviorNode> nodes) : base(nodeName)
        {
            _nodeLeaves = nodes;
        }

        public override NodeStatus Evaluate()
        {
            var anyChildRunning = false;

            foreach (var node in _nodeLeaves)
            {
                switch (node.Evaluate())
                {
                    case NodeStatus.Failed:
                        _nodeStatus = NodeStatus.Failed;
                        return _nodeStatus;
                    case NodeStatus.Success:
                        continue;
                    case NodeStatus.Running:
                        anyChildRunning = true;
                        continue;
                    default:
                        _nodeStatus = NodeStatus.Success;
                        return _nodeStatus;
                }
            }
            _nodeStatus = anyChildRunning ? NodeStatus.Running : NodeStatus.Success;
            return _nodeStatus;
        }
    }
}
