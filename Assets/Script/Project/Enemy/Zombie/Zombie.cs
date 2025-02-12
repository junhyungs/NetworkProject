using UnityEngine;
using CustomBehaviorTree;
using Mirror;
using System;
using System.Collections.Generic;

public class Zombie : NetworkBehaviour, ITakeDamaged
{
    [Header("EnemyData")]
    [SerializeField] private EnemyData _data;

    [Header("HitParticle")]
    [SerializeField] private GameObject _hitParticleObject;
    [SerializeField] private Transform _particleTransform;

    private ParticleSystem _hitParticle;
    private Animator _animator;
    private Action<bool> _behaviorTreeController;
    
    public Transform Target { get; set; }
    public bool IsHit { get; set; }

    private INode _node;

    private readonly int _die = Animator.StringToHash("Die");

    [SyncVar]
    private float _health;
    [SyncVar]
    private float _damage;
    public float Damage => _damage;
    [SyncVar]
    private float _speed;
    public float Speed => _speed;   

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        InstantiateHitParticle();
    }

    private void InstantiateHitParticle()
    {
        if(_hitParticleObject == null)
        {
            return;
        }

        var hitParticle = Instantiate(_hitParticleObject, _particleTransform);
        _hitParticle = hitParticle.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (isServer)
        {
            OnStartZombie();

            _node = SetBehaviorTree();
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

    private INode SetBehaviorTree()
    {
        INode node = new SelectorNode(new List<INode>
        {
            new SelectorNode(new List<INode>
            {
                new ZombieHit(this),
                new ZombieAttack(this),
                new ZombieMove(this),
            }),

            new ZombieCheckTarget(this)

        });

        return node;
    }

    public void ZombieStop()
    {
        if (isServer)
        {
            TakeDamaged(_health);
        }
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
        IsHit = true;

        _health -= damage;

        if(_health <= 0)
        {
            _behaviorTreeController.Invoke(true);
            ClientRPC_DieAnimation();
        }
    }

    [ClientRpc]
    private void ClientRPC_DieAnimation()
    {
        _animator.SetTrigger(_die);
    }
}