using UnityEngine;
using ColliderHandler;

public class ZombieColliderHandler : ColliderHandler<ZombieAnimationEvent>
{
    public override void InitializeHandler(ZombieAnimationEvent tComponent, float damage, int index)
    {
        base.InitializeHandler(tComponent, damage, index);
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
            takeDamaged.TakeDamaged(_damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IsColliding(other, _index);
    }
}
