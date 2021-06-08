namespace GameFramework.AI.SimpleBehaviorTree
{
    public class BinaryRandomNode : BaseBehaviorNode
    {
        private readonly System.Random _rng;

        public BinaryRandomNode(string nodeName) : base(nodeName)
        {
            _rng = new System.Random();
        }

        public override NodeStatus Evaluate()
        {
            var randomNum = _rng.Next(2);

            return randomNum == 0 ? NodeStatus.Success : NodeStatus.Failed;
        }
    }
}
