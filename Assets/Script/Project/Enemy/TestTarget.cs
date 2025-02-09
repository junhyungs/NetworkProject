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

    private void Start()
    {
        var tt = NetworkServer.spawned;

        foreach(var identity in tt.Values)
        {
            Debug.Log($"net id = {identity.netId}, name = {identity.name}, {identity.assetId}, {identity.gameObject.name}");
        }
    }
}
