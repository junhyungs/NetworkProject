using UnityEngine;

public class ZombieHitState : StateMachineBehaviour
{
    private Zombie _zombie;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_zombie == null)
        {
            _zombie = animator.GetComponent<Zombie>();
        }

        _zombie.HitAnimation = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _zombie.HitAnimation = false;
        _zombie.IsHit = false;
    }
}
