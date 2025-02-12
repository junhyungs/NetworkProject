using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public ObjectName ItemName;
    public abstract void UseItem(GamePlayer player);
}