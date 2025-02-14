using UnityEngine;
using CustomBehaviorTree;
using UnityEngine.AI;


public class ZombieCanAttack : ActionNode<Zombie>
{
    public ZombieCanAttack(Zombie baseReference) : base(baseReference)
    {
        _agent = baseReference.GetComponent<NavMeshAgent>();
        _animator = baseReference.GetComponent<Animator>();
    }

    private Animator _animator;
    private NavMeshAgent _agent;
    private Vector3 _targetPosition;
    private Vector3 _myPosition;

    private readonly int _moveAnimation = Animator.StringToHash("Move");

    public override INode.State Evaluate()
    {
        bool isAttack = _baseReference.CanAttack;

        if (!isAttack)
        {
            return INode.State.Runing;
        }

        bool distance = CarculateDistance();

        if (distance)
        {
            Vector3 direction = (_targetPosition - _myPosition).normalized;

            Vector3 forward = _baseReference.transform.forward;

            float angle = Vector3.Angle(forward, direction);

            if(angle > 10f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);

                _baseReference.transform.rotation = Quaternion.Slerp(_baseReference.transform.rotation,
                    rotation, 20f * Time.deltaTime);

                return INode.State.Runing;
            }

            _agent.SetDestination(_baseReference.transform.position);

            _animator.SetBool(_moveAnimation, false);

            return INode.State.Success;
        }

        return INode.State.Fail;
    }

    private bool CarculateDistance()
    {
        _myPosition = _baseReference.transform.position;

        _targetPosition = _baseReference.Target.position;

        return Vector3.Distance(_myPosition, _targetPosition) <= _agent.stoppingDistance;
    }
}
