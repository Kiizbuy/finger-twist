using GameFramework.AI.GOAP;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoToHospital : GoapAction
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private string _targetTag = "Enemy";

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public override void StartAction()
    {
        _agent.SetDestination(_target.position);
    }

    public override void ScheduleAction()
    {
        throw new System.NotImplementedException();
    }

    public override void StopAction()
    {
        _agent.isStopped = true;
    }

    public override bool CanStartAction()
    {
        if (_target == null && _targetTag != string.Empty)
            _target = GameObject.FindWithTag(_targetTag).transform;

        return _target != null;
    }

    public override bool ActionRunning()
    {
        return _actionState == ActionState.Running;
    }

    public override bool ActionHasComplete()
    {
        return _agent.hasPath && _agent.remainingDistance < 1f;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
