using UnityEngine;
using CustomBehaviorTree;
using Mirror;
using UnityEngine.AI;

public class Zombie : NetworkBehaviour, ITakeDamaged
{
    [Header("EnemyData")]
    [SerializeField] private EnemyData _data;

    [Header("HitParticle")]
    [SerializeField] private GameObject _hitParticleObject;
    [SerializeField] private Transform _particleTransform;
    private ParticleSystem _hitParticle;

    private Animator _animator;

    private INode _node;

    private readonly int _die = Animator.StringToHash("Die");

    [SyncVar]
    private float _health;
    [SyncVar]
    private float _damage;
    [SyncVar]
    private float _speed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isServer)
        {
            OnStartZombie();
        }
    }

    private void OnStartZombie()
    {
        if(_data == null)
        {
            return;
        }

        _health = _data.Health;
        _damage = _data.Damage;
        _speed = _data.MoveSpeed;
    }

    private void Update()
    {
        if(_node != null)
        {
            _node.Evaluate();
        }
    }

    private void SetBehaviorTree()
    {

    }

    [ClientRpc]
    public void HitEffect()
    {
        if(_hitParticle != null)
        {
            _hitParticle.Play();
        }
    }

    [Server]
    public void TakeDamaged(float damage)
    {
        _health -= damage;

        if(_health <= 0)
        {
            DieAnimation();
        }
    }

    [ClientRpc]
    private void DieAnimation()
    {
        _animator.SetTrigger(_die);
    }
}

public class ZombieProperty
{
    public Animator Animator { get; set; }
    public NavMeshAgent Agent { get; set; }
}