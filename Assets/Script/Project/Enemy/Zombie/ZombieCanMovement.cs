using UnityEngine;
using CustomBehaviorTree;
using UnityEngine.AI;

public class ZombieCanMovement : ActionNode<Zombie>
{
    public ZombieCanMovement(Zombie baseReference) : base(baseReference)
    {
        _animationEvent = baseReference.GetComponent<ZombieAnimationEvent>();
        _agent = baseReference.GetComponent<NavMeshAgent>();    
    }

    private ZombieAnimationEvent _animationEvent;
    private NavMeshAgent _agent;

    public override INode.State Evaluate()
    {
        if(_animationEvent.PlayHitAnimation ||
            !_animationEvent.CanAttack)
        {
            _agent.SetDestination(_baseReference.transform.position);

            return INode.State.Runing;
        }

        return INode.State.Success;
    }
}
