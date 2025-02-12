using UnityEngine;
using CustomBehaviorTree;

public class ZombieCheckTarget : ActionNode<Zombie>
{
    public ZombieCheckTarget(Zombie baseReference) : base(baseReference) { }
    
    public override INode.State Evaluate()
    {
        if(_baseReference.Target != null)
        {
            return INode.State.Success;
        }

        var newTarget = GameManager.Instance.GetRandomLocalPlayerTransform();

        if(newTarget != null)
        {
            _baseReference.Target = newTarget;

            return INode.State.Success;
        }
        else
        {
            _baseReference.ZombieStop();

            return INode.State.Fail;
        }
    }
}
