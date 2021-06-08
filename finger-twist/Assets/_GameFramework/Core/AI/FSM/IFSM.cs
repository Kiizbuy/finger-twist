using System;

namespace GameFramework.AI
{
    public interface IFSM
    {
        IState Current { get; }

        void AddTransition(IState from, IState to, Func<bool> condition);
        void ChangeState(IState state);
        void ExecuteCurrentState();
        void ResetCurrentState();
        Transition? GetTransition();
    }

    public interface IState
    {
        void OnStart();
        void OnUpdate();
        void OnExit();
        void OnReset();
    }

    public readonly struct Transition
    {
        public readonly IState To;

        public readonly Func<bool> Condition;

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }
}
