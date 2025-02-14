using UnityEngine;
using Mirror;
using CustomBehaviorTree;

public class ZombieAttack : ActionNode<Zombie>
{
    public ZombieAttack(Zombie baseReference) : base(baseReference)
    {
        _animator = baseReference.GetComponent<Animator>();
    }

    private Animator _animator;

    private readonly int _attack = Animator.StringToHash("Attack");

    public override INode.State Evaluate()
    {
        _animator.SetTrigger(_attack);

        ClientRpc_Attack();

        return INode.State.Runing;
    }

    [ClientRpc]
    private void ClientRpc_Attack()
    {
        _animator.SetTrigger(_attack);
    }
}
