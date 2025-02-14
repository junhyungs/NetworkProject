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
        bool isMove = Vector3.Distance(_baseReference.Target.position,
            _baseReference.transform.position) > _agent.stoppingDistance;

        Vector3 targetPosition = isMove ? _baseReference.Target.position :
            _baseReference.transform.position;
        
        _agent.SetDestination(targetPosition);

        ClientRpc_Movement(isMove);

        return INode.State.Runing;
    }

    [ClientRpc]
    private void ClientRpc_Movement(bool isMove)
    {
        _animator.SetBool(_moveAnimation, isMove);
    }
}
