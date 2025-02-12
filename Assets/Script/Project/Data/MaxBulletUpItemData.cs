using UnityEngine;

[CreateAssetMenu(fileName = "MaxBulletItemData", menuName = "Scriptable Data/MaxBulletItemData")]
public class MaxBulletUpItemData : ItemData
{
    [SerializeField] private int _maxBullet;

    public override void UseItem(GamePlayer player)
    {
        player.SyncMaxBullet += _maxBullet;
    }
}
