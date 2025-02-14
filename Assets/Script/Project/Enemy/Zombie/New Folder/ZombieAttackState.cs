using UnityEngine;

public class ZombieAttackState : StateMachineBehaviour
{
    private Zombie _zombie;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_zombie == null)
        {
            _zombie = animator.GetComponent<Zombie>();
        }

        _zombie.CanAttack = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _zombie.CanAttack = true;
    }
}
