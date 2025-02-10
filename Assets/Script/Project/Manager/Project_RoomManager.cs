using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class Project_RoomManager : NetworkRoomManager
{
    private Dictionary<DataKey, ScriptableObject> _dataDictionary = new Dictionary<DataKey, ScriptableObject>();    
    private readonly object _lock = new object();
    public event Action<int> _readyPlayerCountCallBack;
   
    /// <summary>
    /// isServer
    /// </summary>
    private int _readyPlayerCount;
    public int ReadyPlayerCount => _readyPlayerCount;

    public void SetReadyPlayerCount(bool isReady)
    {
        _readyPlayerCount += isReady ? 1 : -1;

        _readyPlayerCountCallBack.Invoke(_readyPlayerCount);
    }

    public static Project_RoomManager Instance
    {
        get
        {
            if(singleton != null)
            {
                var instance = singleton as Project_RoomManager;

                return instance;
            }

            var roomManager = FindFirstObjectByType<Project_RoomManager>();

            if(roomManager != null)
            {
                return roomManager;
            }
            
            throw new System.Exception("RoomManager is Null");
        }
    }

    public void AddData(DataKey key, ScriptableObject data)
    {
        lock (_lock)
        {
            if (!_dataDictionary.ContainsKey(key))
            {
                _dataDictionary.Add(key, data);
            }
        }
    }

    public T GetData<T>(DataKey key) where T : class
    {
        if (!_dataDictionary.ContainsKey(key))
        {
            return null;
        }

        var data = _dataDictionary[key];

        return data is T Tdata ? Tdata : null;
    }

    public void StopGame()
    {
        if(NetworkServer.active && NetworkClient.active)
        {
            singleton.StopHost();
        }
        else if(NetworkClient.active)
        {
            singleton.StopClient();
        }
        else if(NetworkServer.active)
        {
            singleton.StopServer();
        }

        SceneManager.LoadScene("LoginScene");
    }

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        ConnectionCount();
    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        ConnectionCount();
    }

    private void ConnectionCount()
    {
        var connectionCount = NetworkServer.connections.Count;
        
        var gameUIManager = GameUIManager.Instance;

        if(gameUIManager.PlayerCountUI != null)
        {
            gameUIManager.PlayerCountUI._currentPlayerCount = connectionCount;
        }
    }

    /// <summary>
    /// 씬이 전환되면 호출되는 메서드.
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == "Assets/Scenes/ProjectScene.unity")
        {
            ConnectionCount();
        }
    }

    /// <summary>
    /// OnServerSceneChanged -> if(sceneName != RoomScene) == true -> SceneLoadedForPlayer
    /// 메서드 내부에서 GameScenePlayer 생성 후 호출. (초기화 목적)
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns></returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        var roomPlayerComponent = roomPlayer.GetComponent<Project_RoomPlayer>();
        var gamePlayerComponent = gamePlayer.GetComponent<GamePlayer>();

        if(roomPlayerComponent == null || gamePlayerComponent == null)
        {
            return false;
        }

        var data = GetData<PlayerData>(DataKey.Player);
        gamePlayerComponent.PlayerData = data;

        return true;
    }

    /// <summary>
    /// GameScene에서 생성되는 Player의 위치.
    /// </summary>
    /// <returns></returns>
    public override Transform GetStartPosition()
    {
        var playerTransform = FindFirstObjectByType<PlayerSpawnPosition>();

        if(playerTransform == null)
        {
            return null;
        }

        var randomTransform = playerTransform.GetSpawnTransform();

        return randomTransform;
    }
}
