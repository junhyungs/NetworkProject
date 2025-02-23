using UnityEngine;
using Mirror;
using System.Collections.Generic;

[System.Serializable]
public struct PartyPlayer
{
    public uint NetId;
    public string Name;
    public float Health;

    public PartyPlayer(string name, float health, uint netId)
    {
        Name = name;
        Health = health;
        NetId = netId;
    }
}

public class NetworkUIManager : NetworkBehaviour
{
    private static NetworkUIManager _instance;
    public static NetworkUIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<NetworkUIManager>();
            }
            
            return _instance;
        }
    }

    private Dictionary<uint, PartyPlayer> _serverPartyDictionary = new Dictionary<uint, PartyPlayer>();
    private object _lock = new object();

    [Server]
    public void RegisterParty(uint netId, string name)
    {
        lock(_lock)
        {
            if (_serverPartyDictionary.ContainsKey(netId))
            {
                return;
            }

            var playerIdentity = NetworkServer.spawned[netId];
            var player = playerIdentity.GetComponent<Player>();

            float health = player.SyncHealth;

            var partyPlayer = new PartyPlayer(name, health, netId);
            _serverPartyDictionary.Add(netId, partyPlayer);


            foreach (var partyData in _serverPartyDictionary.Values)
            {
                ClientRpc_MakePartyUI(partyData);
            }
        }
        
    }

    [Server]
    public void UnregisterParty(uint netId)
    {
        lock(_lock)
        {
            if (_serverPartyDictionary.ContainsKey(netId))
            {
                _serverPartyDictionary.Remove(netId);

                ClientRpc_DeletePartyUI(netId);
            }
        }
    }

    [Server]
    public void UpdatePartyUI(uint netId, float health)
    {
        ClientRpc_UpdataPartyUI(netId, health);
    }

    [ClientRpc]
    public void ClientRpc_MakePartyUI(PartyPlayer partyPlayerData)
    {
        UIManager.Instance.MakePartyUI(partyPlayerData);
    }

    [ClientRpc]
    public void ClientRpc_UpdataPartyUI(uint netId, float health)
    {
        UIManager.Instance.UpdatePartyUI(netId, health);
    }

    [ClientRpc]
    public void ClientRpc_DeletePartyUI(uint netId)
    {
        UIManager.Instance.DeletePartyUI(netId);
    }

    private void OnDestroy()
    {
        if (_instance != null)
        {
            _instance = null;
        }
    }

    public void OnReadyUI(bool isOn)
    {
        ClientRpc_OnReadyUI(isOn);
    }

    [ClientRpc]
    private void ClientRpc_OnReadyUI(bool isOn)
    {
        UIManager.Instance.OnReadyUI(isOn);
    }

    public void SetPlayerConnectionCount()
    {
        var count = NetworkServer.connections.Count;

        ClientRpc_PlayerConnectionCount(count);
    }

    [ClientRpc]
    private void ClientRpc_PlayerConnectionCount(int count)
    {
        UIManager.Instance.ConnectionCount(count);
    }
}
