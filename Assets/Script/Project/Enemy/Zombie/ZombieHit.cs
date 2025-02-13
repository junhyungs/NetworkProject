using UnityEngine;
using CustomBehaviorTree;

public class ZombieHit : ActionNode<Zombie>
{
    public ZombieHit(Zombie baseReference) : base(baseReference)
    {
        _animator = baseReference.GetComponent<Animator>();
    }

    private Animator _animator;

    private readonly int _hit = Animator.StringToHash("Hit");

    public override INode.State Evaluate()
    {
        if (!_baseReference.IsHit)
        {
            return INode.State.Fail;
        }

        _animator.SetTrigger(_hit);

        return INode.State.Success;
    }
}
