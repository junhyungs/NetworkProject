using UnityEngine;
using CustomBehaviorTree;

public class ZombieIsTarget : ActionNode<Zombie>
{
    public ZombieIsTarget(Zombie baseReference) : base(baseReference) { }
    
    public override INode.State Evaluate()
    {
        bool targetNull = _baseReference.Target == null;

        if (targetNull)
        {
            var newTarget = GameManager.Instance.GetRandomLocalPlayerTransform();

            if(newTarget != null )
            {
                _baseReference.Target = newTarget;
            }
            else
            {
                return INode.State.Runing;
            }
        }

        return INode.State.Fail;
    }
}
