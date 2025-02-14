using UnityEngine;
using System.Collections.Generic;

public class ZombieAnimationEvent : MonoBehaviour
{
    [Header("TargetObject")]
    [SerializeField] private GameObject[] _targetObjects;

    private Zombie _zombie;
    private Animator _animator;
    private SphereCollider[] _attackColliders;
    private HashSet<int> _overlapSet = new HashSet<int>();

    private string _findClip = "Die";

    public HashSet<int> OverlapSet => _overlapSet;
    public bool IsServer
    {
        get
        {
            return _zombie.isServer;
        }
    }

    public float Damage
    {
        get
        {
            return _zombie.Damage;
        }
    }

    public float DieAnimationLength { get; set; }
    
    private void Awake()
    {
        _zombie = GetComponent<Zombie>();

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_zombie.isServer)
        {
            AddComponent();

            DieAnimationLength = GetAnimationLength(_findClip);
        }
    }

    private float GetAnimationLength(string clipName)
    {
        if(_animator == null)
        {
            return 0f;
        }

        foreach(var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0f;
    }

    private void AddComponent()
    {
        if(_targetObjects == null)
        {
            return;
        }

        _attackColliders = new SphereCollider[_targetObjects.Length];

        for(int i = 0; i < _targetObjects.Length; i++)
        {
            var targetObject = _targetObjects[i];

            SphereCollider sphereCollider = targetObject.AddComponent<SphereCollider>();
            sphereCollider.enabled = true;
            sphereCollider.isTrigger = true;

            _attackColliders[i] = sphereCollider;

            ZombieColliderHandler handler = targetObject.AddComponent<ZombieColliderHandler>();
            handler.InitializeHandler(this, _zombie.Damage, 1);
        }
    }
}

