using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : NetworkSingleton<GameManager>
{
    private Dictionary<int, NetworkIdentity> _localPlayerDictionary = new Dictionary<int, NetworkIdentity>(); 
    private List<NetworkIdentity> _deathPlayerList = new List<NetworkIdentity>();
    private List<NetworkIdentity> _alivePlayerList;
    private readonly object _lockObject = new object();
    private bool _isGameOver;

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
        _alivePlayerList = new List<NetworkIdentity>(_localPlayerDictionary.Values);
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
                _alivePlayerList.Remove(identity);

                _localPlayerDictionary.Remove(connectionId);
            }
        }
    }

    [Server]
    public Transform GetRandomLocalPlayerTransform()
    {
        lock (_lockObject)
        {
            if(_alivePlayerList.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, _alivePlayerList.Count);

            var networkIdentity = _alivePlayerList[randomIndex];

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

    public void ReGame()
    {
        _isGameOver = false;

        RespawnPlayer();

        _spawnSystem.ReGameSpawnSystem();

        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        while (!_isGameOver)
        {
            _spawnSystem.StartSpawnEnemy();

            yield return new WaitUntil(() =>
            {
                return _spawnSystem.WaveClear;
            });

            if (!_isGameOver)
            {
                RespawnPlayer();
            }
        }
    }

    [Server]
    public void RespawnPlayer()
    {
        var randomTransform = FindFirstObjectByType<PlayerSpawnPosition>();

        foreach(var networkIdentity in _deathPlayerList)
        {
            GamePlayer gamePlayer = networkIdentity.GetComponent<GamePlayer>();

            if(randomTransform != null)
            {
                var transform = randomTransform.GetSpawnTransform();

                networkIdentity.transform.position = transform.position;
            }

            gamePlayer.ClientRpc_RespawnPlayer();
            gamePlayer.SyncHealth = 100f;
            gamePlayer.TargetRpc_SetDeathLayer(false);

            if (!_alivePlayerList.Contains(networkIdentity))
            {
                _alivePlayerList.Add(networkIdentity);   
            }
        }

        _deathPlayerList.Clear();
    }

    [Server]
    public void SetDeathPlayer(NetworkIdentity identity)
    {
        lock(_lockObject)
        {
            bool isIdentity = identity != null;

            if (!_deathPlayerList.Contains(identity) && isIdentity)
            {
                _deathPlayerList.Add(identity);

                _alivePlayerList.Remove(identity);

                if(_deathPlayerList.Count == _localPlayerDictionary.Count)
                {
                    _spawnSystem.StopSpawnEnemy();

                    _isGameOver = true;

                    UIManager.Instance.GameUI.OnRegameButton();

                    Debug.Log("게임 종료");
                }
            }
        }
    }
    #endregion
}
