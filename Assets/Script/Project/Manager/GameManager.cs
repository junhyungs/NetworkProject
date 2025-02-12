using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : NetworkSingleton<GameManager>
{
    private Dictionary<int, NetworkIdentity> _localPlayerDictionary = new Dictionary<int, NetworkIdentity>(); 
    private List<NetworkIdentity> _deathPlayerList = new List<NetworkIdentity>();
    private List<NetworkIdentity> _localPlayerList;
    private readonly object _lockObject = new object();
    private bool _isClientReady;
    public bool IsClientReady => _isClientReady;

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(ClientReady());
        }
    }

    private IEnumerator ClientReady()
    {
        var connectionCount = NetworkServer.connections.Count;

        yield return new WaitUntil(() =>
        {
            return connectionCount == _localPlayerDictionary.Count;
        });

        _isClientReady = true;

        _localPlayerList = new List<NetworkIdentity>(_localPlayerDictionary.Values);
    }

    public void AddLocalPlayer(int connectionId,  NetworkIdentity identity)
    {
        lock(_lockObject)
        {
            if (!_localPlayerDictionary.ContainsKey(connectionId))
            {
                _localPlayerDictionary.Add(connectionId, identity);
            }
        }
    }
    
    public void RemoveLocalPlayer(int connectionId)
    {
        lock(_lockObject)
        {
            if (_localPlayerDictionary.ContainsKey(connectionId))
            {
                _localPlayerDictionary.Remove(connectionId);
            }
        }
    }

    public Transform GetRandomLocalPlayerTransform()
    {
        lock (_lockObject)
        {
            if(_localPlayerList.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, _localPlayerList.Count);

            var randomTransform = _localPlayerList[randomIndex].transform;

            return randomTransform;
        }
    }


}
