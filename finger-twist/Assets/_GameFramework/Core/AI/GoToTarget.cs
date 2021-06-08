using GameFramework.AI.GOAP;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoToTarget : GoapAction
{
    [SerializeField] private float _stopDistance = 5f;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private string _targetTag = "Enemy";

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public override bool IsAchievable()
    {
        return CanStartAction();
    }

    public override void StartAction()
    {
        _agent.isStopped = false;
        _actionState = ActionState.Running;
    }

    public override void ScheduleAction()
    {
        _agent.SetDestination(_target.position);
    }

    public override void StopAction()
    {
        _agent.isStopped = true;
        _actionState = ActionState.Idle;
    }

    public override bool CanStartAction()
    {
        if (_target == null && _targetTag != string.Empty)
            _target = GameObject.FindWithTag(_targetTag).transform;

        var dist = (transform.position - _target.position).sqrMagnitude;

        return _target != null && dist > _stopDistance * _stopDistance;
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
