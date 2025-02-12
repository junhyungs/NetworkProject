using UnityEngine;

[CreateAssetMenu(fileName = "MoveSpeedUpItemData", menuName = "Scriptable Data/MoveSpeedUpItemData")]
public class MoveSpeedUpItemData : ItemData
{
    [SerializeField] private float _moveSpeed;

    public override void UseItem(GamePlayer player)
    {
        player.SyncMovespeed += _moveSpeed;
    }
}
