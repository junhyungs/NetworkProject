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
    private NavMeshAgent _agent;
    private Animator _animator;

    private Action<bool> _behaviorTreeController;
    private Action<Zombie> _returnCallBack;
    
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
        _agent = GetComponent<NavMeshAgent>();
        _animationEvent = GetComponent<ZombieAnimationEvent>();

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
            _node = SetBehaviorTree();
        }
    }

    private void Update()
    {
        if(_node != null)
        {
            _node.Evaluate();
        }
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
            _returnCallBack?.Invoke(this);

            _behaviorTreeController.Invoke(true);

            var dieLength = _animationEvent.DieAnimationLength;

            StartCoroutine(ReturnZombie(dieLength));

            ClientRPC_DieAnimation();
        }
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
        _animator.SetTrigger(_die);
    }
}