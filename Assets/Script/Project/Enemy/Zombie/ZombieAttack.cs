using UnityEngine;
using Mirror;
using CustomBehaviorTree;

public class ZombieAttack : ActionNode<Zombie>
{
    public ZombieAttack(Zombie baseReference) : base(baseReference) { }
    
    public override INode.State Evaluate()
    {       
        _baseReference.ClientRpc_AttackAnimation();

        return INode.State.Runing;
    }
}
