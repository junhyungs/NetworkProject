using UnityEngine;
using CustomBehaviorTree;
using Mirror;
using UnityEngine.AI;

public class ZombieHit : ActionNode<Zombie>
{
    public ZombieHit(Zombie baseReference) : base(baseReference)
    {
        _agent = baseReference.GetComponent<NavMeshAgent>();
    }

    private NavMeshAgent _agent;

    public override INode.State Evaluate()
    {
        bool ishit = _baseReference.IsHit;
        bool hitAnimation = _baseReference.HitAnimation;

        if(ishit && !hitAnimation)
        {
            _agent.SetDestination(_baseReference.transform.position); 

            _baseReference.ClientRpc_HitAnimation();

            return INode.State.Runing;
        }
        
        return INode.State.Fail;
    }
}
