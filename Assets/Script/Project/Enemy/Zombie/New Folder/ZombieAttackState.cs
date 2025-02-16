using UnityEngine;

public class ZombieAttackState : StateMachineBehaviour
{
    private Zombie _zombie;
    private ZombieAnimationEvent _zombieAnimationEvent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_zombie == null)
        {
            _zombie = animator.GetComponent<Zombie>();
            _zombieAnimationEvent = animator.GetComponent<ZombieAnimationEvent>();
        }

        _zombie.CanAttack = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _zombieAnimationEvent.ResetHashSet();
        _zombie.CanAttack = true;
    }
}
