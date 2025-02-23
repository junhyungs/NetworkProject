using UnityEngine;
using Mirror;

public class PlayerGun : NetworkBehaviour
{
    [Header("FireParticle")]
    [SerializeField] private ParticleSystem _fireParticle;
    [Header("Bullet")]
    [SerializeField] private ParticleSystem _bulletParticle;
    [Header("FirePosition")]
    [SerializeField] private Transform _fireTransform;

    private LayerMask _targetLayer;

    private void Awake()
    {
        if(_fireParticle != null)
        {
            var mainSystem = _fireParticle.main;

            mainSystem.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        if(_bulletParticle != null)
        {
            var mainSystem = _bulletParticle.main;

            mainSystem.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        var player = GetComponentInParent<Player>();

        if(player != null)
        {
            player.Gun = this;
        }
    }

    private void Start()
    {
         _targetLayer = LayerMask.GetMask("Enemy");
    }

    public void Fire()
    {
        if (isOwned)
        {
            FireRayCast();
            CommandFire();
        }
    }

    private void FireRayCast()
    {
        Vector3 rayDirection = _fireTransform.forward;

        Debug.DrawRay(_fireTransform.position, rayDirection * 200f, Color.red, 10f);

        if (Physics.Raycast(_fireTransform.position, rayDirection, out RaycastHit hit, 200f, _targetLayer))
        {
            NetworkIdentity identity = hit.collider.GetComponent<NetworkIdentity>();

            if(identity != null)
            {
                CommandAttack(identity.netId, 10f);
            }
        }
    }

    [Command]
    private void CommandAttack(uint identityId, float damage)
    {
        if(!NetworkServer.spawned.ContainsKey(identityId))
        {
            return;
        }

        NetworkIdentity identity = NetworkServer.spawned[identityId];

        ITakeDamaged takeDamaged = identity.GetComponent<ITakeDamaged>();

        if (takeDamaged != null)
        {
            takeDamaged.TakeDamaged(damage);
            takeDamaged.HitEffect();
        }
    }

    [Command]
    private void CommandFire()
    {
        ClientRpcFire();
    }

    [ClientRpc]
    private void ClientRpcFire()
    {
        _fireParticle.Play();

        _bulletParticle.Play();
    }
}
