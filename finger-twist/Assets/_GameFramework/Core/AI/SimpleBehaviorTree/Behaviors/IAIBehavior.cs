namespace GameFramework.AI.SimpleBehaviorTree
{
    public interface IAIBehavior
    {
        float GetCost { get; }
        void UpdateState();
        void Evaluate();
        bool TryBehave();
    }
}
