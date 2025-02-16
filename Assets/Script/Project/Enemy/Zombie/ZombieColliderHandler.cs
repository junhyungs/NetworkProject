using UnityEngine;
using ColliderHandler;

public class ZombieColliderHandler : ColliderHandler<ZombieAnimationEvent>
{
    public override void InitializeHandler(ZombieAnimationEvent tComponent, int index)
    {
        base.InitializeHandler(tComponent, index);
    }

    protected override void IsColliding(Collider other, int index)
    {
        if (!_tComponent.IsServer)
        {
            return;
        }

        if (_tComponent.OverlapSet.Contains(index))
        {
            return;
        }

        _tComponent.OverlapSet.Add(index);

        ITakeDamaged takeDamaged = other.GetComponent<ITakeDamaged>();

        if (takeDamaged != null)
        {
            takeDamaged.TakeDamaged(_tComponent.Damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.gameObject.layer == LayerMask.NameToLayer("Player");

        if (isPlayer)
        {
            GamePlayer player = other.gameObject.GetComponent<GamePlayer>();

            float currentHP = player.SyncHealth;

            if (currentHP <= 0f)
            {
                _tComponent.Zombie.Target = null;
            }

            IsColliding(other, _index);
        }
    }
}
