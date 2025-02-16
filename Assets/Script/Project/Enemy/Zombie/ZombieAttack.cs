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
    //애니메이션 트리거는 NetworkAnimator가 자동으로 동기화 하지 않는다.
    //따라서 Rpc로 동기화해줘야함.
    public override INode.State Evaluate()
    {
        _animator.SetTrigger(_attack);

        ClientRpc_Attack();

        return INode.State.Runing;
    }

    [ClientRpc]
    private void ClientRpc_Attack()
    {
        if (!_baseReference.isServer)
        {
            _animator.SetTrigger(_attack);
        }
    }
}
