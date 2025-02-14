using UnityEngine;
using CustomBehaviorTree;
using Mirror;
using UnityEngine.AI;

public class ZombieHit : ActionNode<Zombie>
{
    public ZombieHit(Zombie baseReference) : base(baseReference)
    {
        _animator = baseReference.GetComponent<Animator>();
        _agent = baseReference.GetComponent<NavMeshAgent>();
    }

    private Animator _animator;
    private NavMeshAgent _agent;
    private readonly int _hit = Animator.StringToHash("Hit");
    private readonly int _attack = Animator.StringToHash("Attack");

    public override INode.State Evaluate()
    {
        bool ishit = _baseReference.IsHit;
        bool hitAnimation = _baseReference.HitAnimation;

        if(ishit && !hitAnimation)
        {
            _agent.SetDestination(_baseReference.transform.position);

            ClientRpc_HitAnimation();

            return INode.State.Runing;
        }
        
        return INode.State.Fail;
    }

    [ClientRpc]
    private void ClientRpc_HitAnimation()
    {
        _animator.ResetTrigger("Attack");

        _animator.SetTrigger(_hit);
    }
}
