using UnityEngine;
using Mirror;
public class TestTarget : NetworkBehaviour, ITakeDamaged
{
    [SerializeField] private GameObject _hitParticle;
    [SerializeField] private Transform _particleTransform;

    private ParticleSystem _particleSystem;

    [SyncVar(hook = nameof(Health_Hook))]
    private float _health;
    private void Health_Hook(float _, float health)
    {
        Debug.Log(health);
    }

    [ClientRpc]
    public void HitEffect()
    {
        _particleSystem.Play();
    }

    [Server]
    public void TakeDamaged(float damage)
    {
        _health -= damage;
    }

    private void Awake()
    {
        var hitParticleObject = Instantiate(_hitParticle, _particleTransform);

        _particleSystem = hitParticleObject.GetComponent<ParticleSystem>();
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            _health = 100f;
        }
    }
}
