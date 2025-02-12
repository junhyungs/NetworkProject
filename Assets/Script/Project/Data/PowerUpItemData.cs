using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpItemData", menuName = "Scriptable Data/PowerUpItemData")]
public class PowerUpItemData : ItemData
{
    [SerializeField] private float _power;

    public override void UseItem(GamePlayer player)
    {
        player.SyncDamage += _power;
    }
}
