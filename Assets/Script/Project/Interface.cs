using System;
using UnityEngine;
using Mirror;

public interface ITakeDamaged
{
    [Server]
    public void TakeDamaged(float damage);

    [ClientRpc]
    public void HitEffect();
}

public interface IItemReturnEvent
{
    public void SetEvent(Action<Transform> callBack);
}

public interface INode
{
    public enum State
    {
        Success,
        Fail,
        Runing
    }

    public State Evaluate();
}