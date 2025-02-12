using UnityEngine;
using CustomBehaviorTree;
using UnityEngine.AI;
using Mirror;

public class ZombieMove : ActionNode<Zombie>
{
    public ZombieMove(Zombie baseReference) : base(baseReference)
    {
        _animator = baseReference.GetComponent<Animator>();
        _agent = baseReference.GetComponent<NavMeshAgent>();

        _agent.speed = baseReference.Speed;
    }

    private Animator _animator;
    private NavMeshAgent _agent;

    private readonly int _moveAnimation = Animator.StringToHash("Move");

    public override INode.State Evaluate()
    {
        if(_baseReference.Target == null)
        {
            return INode.State.Fail;
        }

        if (_baseReference.IsHit)
        {
            _agent.SetDestination(_baseReference.transform.position);

            return INode.State.Runing;
        }

        bool isMove = Vector3.Distance(_baseReference.transform.position,
            _baseReference.Target.position) > _agent.stoppingDistance;

        if (isMove)
        {
            ClientRpc_Movement(isMove);

            return INode.State.Runing;
        }

        ClientRpc_Movement(isMove);

        return INode.State.Success;
    }

    [ClientRpc]
    private void ClientRpc_Movement(bool isMove)
    {
        _animator.SetBool(_moveAnimation, isMove);

        Vector3 targetPosition = isMove ? _baseReference.Target.position :
            _baseReference.transform.position;

        _agent.SetDestination(targetPosition);
    }
}
