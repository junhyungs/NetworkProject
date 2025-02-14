using UnityEngine;
using CustomBehaviorTree;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class Zombie : NetworkBehaviour, ITakeDamaged
{
    [Header("HitParticle")]
    [SerializeField] private GameObject _hitParticleObject;
    [SerializeField] private Transform _particleTransform;

    private ParticleSystem _hitParticle;
    private ZombieAnimationEvent _animationEvent;
    private CapsuleCollider _capsuleCollider;
    private NavMeshAgent _agent;
    private Animator _animator;

    private Action<Zombie> _returnCallBack;
    
    public Transform Target { get; set; }
    public bool IsHit { get; set; } = false;
    public bool CanAttack { get; set; } = true;
    public bool HitAnimation { get; set; } = false;

    private bool _isDead;
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
        _agent = GetComponent<NavMeshAgent>();
        _animationEvent = GetComponent<ZombieAnimationEvent>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        InstantiateHitParticle();
    }

    private void InstantiateHitParticle()
    {
        if (_hitParticleObject == null)
        {
            return;
        }

        var hitParticle = Instantiate(_hitParticleObject, _particleTransform);
        _hitParticle = hitParticle.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _node = SetBehaviorTree();
    }

    private void Update()
    {
        if(!_isDead)
        {
            _node.Evaluate();
        }
    }

    public void SettingZombie()
    {
        _isDead = false;

        _agent.isStopped = false;

        ClientRpc_ZombieCollider(true);
    }

    public void UnRegisterCallBack(Action<Zombie> callback)
    {
        Action<Zombie> oneTimeAction = null; //일회성 콜백 사용.

        oneTimeAction = (zombie) =>
        {
            callback.Invoke(zombie);

            _returnCallBack -= oneTimeAction;
        };

        _returnCallBack += oneTimeAction;

    }

    private INode SetBehaviorTree()
    {
        INode node = new SelectorNode<Zombie>(new List<INode>()
        {
            new ZombieIsTarget(this),

            new ZombieHit(this),

            new SequenceNode<Zombie>(new List<INode>()
            {
                new ZombieCanAttack(this),
                new ZombieAttack(this)
            }),

             new ZombieMove(this)
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

    [Server]
    public void SetZombieData(float health, float damage, float speed)
    {
        _health = health;
        _damage = damage;
        _speed = speed;  

        _agent.speed = _speed;
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
            _isDead = true;

            Death();

            _returnCallBack?.Invoke(this);
        }
    }

    private void Death()
    {
        _agent.SetDestination(transform.position);
        _agent.isStopped = true;

        ClientRpc_ZombieCollider(false);
        ClientRPC_DieAnimation();

        var dieLength = _animationEvent.DieAnimationLength;

        StartCoroutine(ReturnZombie(dieLength));
    }


    private IEnumerator ReturnZombie(float length)
    {
        yield return new WaitForSeconds(length);

        NetworkServer.UnSpawn(gameObject);        

        ObjectPool.Instance.EnqueuePool(gameObject);
    }

    [ClientRpc]
    private void ClientRPC_DieAnimation()
    {
        _animator.Play("Die");
    }

    [ClientRpc]
    private void ClientRpc_ZombieCollider(bool enabled)
    {
        _capsuleCollider.enabled = enabled;
    }
}