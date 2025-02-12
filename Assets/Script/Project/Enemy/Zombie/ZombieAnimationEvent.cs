using UnityEngine;
using System.Collections.Generic;

public class ZombieAnimationEvent : MonoBehaviour
{
    [Header("TargetObject")]
    [SerializeField] private GameObject[] _targetObjects;

    private Zombie _zombie;
    private SphereCollider[] _attackColliders;
    private HashSet<int> _overlapSet = new HashSet<int>();

    public HashSet<int> OverlapSet => _overlapSet;
    public bool IsServer
    {
        get
        {
            return _zombie.isServer;
        }
    }

    private void Awake()
    {
        _zombie = GetComponent<Zombie>();
    }

    private void Start()
    {
        if (_zombie.isServer)
        {
            AddComponent();
        }
    }

    private void AddComponent()
    {
        if(_targetObjects == null)
        {
            return;
        }

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

    public bool CanAttack { get; set; }

    public void OnAttack()
    {
        CanAttack = true;
    }

    public void OffAttack()
    {
        CanAttack = false;
    }

    public void OffHit()
    {
        _zombie.IsHit = false;
    }
}

