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