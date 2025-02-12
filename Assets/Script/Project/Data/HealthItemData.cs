using UnityEngine;

[CreateAssetMenu(fileName = "HealthItemData", menuName = "Scriptable Data/HealthItemData")]
public class HealthItemData : ItemData
{
    [SerializeField] private float _health;

    public override void UseItem(GamePlayer player)
    {
        player.SyncHealth = Mathf.Clamp(player.SyncHealth + _health, 0f, 100f);
    }
}
