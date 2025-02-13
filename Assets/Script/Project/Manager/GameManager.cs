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

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(ClientReady());
        }
    }

    [Server]
    private IEnumerator ClientReady()
    {
        var connectionCount = NetworkServer.connections.Count;

        NetworkUIManager_OnReadyUI(true);

        yield return new WaitUntil(() =>
        {
            return connectionCount == _localPlayerDictionary.Count && _spawnSystem.SpawnSystemReady;
        });

        LocalPlayerDictionaryToList();

        yield return new WaitForSeconds(2f);

        NetworkUIManager_OnReadyUI(false);

        StartCoroutine(StartWave());
    }

    private void NetworkUIManager_OnReadyUI(bool isOn)
    {
        NetworkUIManager.Instance.OnReadyUI(isOn);
    }

    private void LocalPlayerDictionaryToList()
    {
        _localPlayerList = new List<NetworkIdentity>(_localPlayerDictionary.Values);
    }

    [Server]
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

    [Server]
    public void RemoveLocalPlayer(int connectionId)
    {
        lock(_lockObject)
        {
            if (_localPlayerDictionary.TryGetValue(connectionId, out NetworkIdentity identity))
            {
                _localPlayerList.Remove(identity);

                _localPlayerDictionary.Remove(connectionId);
            }
        }
    }

    [Server]
    public Transform GetRandomLocalPlayerTransform()
    {
        lock (_lockObject)
        {
            if(_localPlayerList.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, _localPlayerList.Count);

            var networkIdentity = _localPlayerList[randomIndex];

            if(networkIdentity != null && networkIdentity.TryGetComponent(out Transform transform))
            {
                return transform;
            }

            return null;
        }
    }

    #region SpawnSystem
    [Header("SpawnSystem")]
    [SerializeField] private SpawnSystem _spawnSystem;

    private IEnumerator StartWave()
    {
        while (true)
        {
            _spawnSystem.StartSpawnEnemy();

            yield return new WaitUntil(() =>
            {
                return _spawnSystem.WaveClear;
            });
        }
    }

    public void SetDeathPlayer(NetworkIdentity identity)
    {
        lock(_lockObject)
        {
            bool isIdentity = identity != null;

            if (!_deathPlayerList.Contains(identity) && isIdentity)
            {
                _deathPlayerList.Add(identity);

                if(_deathPlayerList.Count == _localPlayerList.Count)
                {
                    _spawnSystem.StopSpawnEnemy();

                    Debug.Log("게임 종료");
                }
            }
        }
    }
    #endregion
}
