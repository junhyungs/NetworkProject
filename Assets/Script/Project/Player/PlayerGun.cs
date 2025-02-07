using UnityEngine;
using Mirror;

public class PlayerGun : NetworkBehaviour
{
    [Header("FireParticle")]
    [SerializeField] private ParticleSystem _fireParticle;
    [Header("Bullet")]
    [SerializeField] private ParticleSystem _bulletParticle;

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

    public void Fire()
    {
        if (isOwned)
        {
            CommandFire();
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
