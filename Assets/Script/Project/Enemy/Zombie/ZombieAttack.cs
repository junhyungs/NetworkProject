using UnityEngine;
using Mirror;
using CustomBehaviorTree;
using UnityEngine.AI;

public class ZombieAttack : ActionNode<Zombie>
{
    public ZombieAttack(Zombie baseReference) : base(baseReference)
    {
        _animator = baseReference.GetComponent<Animator>();
        _agent = baseReference.GetComponent<NavMeshAgent>();
        _animationEvent = baseReference.GetComponent<ZombieAnimationEvent>();
    }

    private Animator _animator;
    private ZombieAnimationEvent _animationEvent;
    private NavMeshAgent _agent;

    private readonly int _attack = Animator.StringToHash("Attack");

    public override INode.State Evaluate()
    {
        if (!_animationEvent.CanAttack || _baseReference.IsHit)
        {
            return INode.State.Runing;
        }

        bool distance = CarculateDistance();
        bool rotation = CarculateRotation();

        if(distance && rotation)
        {
            ClientRpc_Attack();

            return INode.State.Success;
        }

        return INode.State.Fail;
    }

    private bool CarculateDistance()
    {
        return Vector3.Distance(_baseReference.transform.position,
            _baseReference.Target.position) <= _agent.stoppingDistance;
    }

    private bool CarculateRotation()
    {
        Vector3 targetDirection = (_baseReference.Target.position
            - _baseReference.transform.position).normalized;  

        Vector3 forward = _baseReference.transform.forward;

        float angle = Vector3.Angle(forward, targetDirection);

        return angle <= 10f;
    }

    [ClientRpc]
    private void ClientRpc_Attack()
    {
        _animator.SetTrigger(_attack);
    }
}
