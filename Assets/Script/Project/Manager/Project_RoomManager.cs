using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class Project_RoomManager : NetworkRoomManager
{
    private Dictionary<string, GameObject> _registerPrefabDictionary;
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

    public override void OnStartServer()
    {
        _registerPrefabDictionary = new Dictionary<string, GameObject>();

        foreach(var prefab in spawnPrefabs)
        {
            var prefabName = prefab.name;

            if (!_registerPrefabDictionary.ContainsKey(prefabName))
            {
                _registerPrefabDictionary.Add(prefabName, prefab);
            }
        }
    }

    public GameObject GetRegisterPrefab(string gameObjectName)
    {
        if (_registerPrefabDictionary.ContainsKey(gameObjectName))
        {
            return _registerPrefabDictionary[gameObjectName];
        }

        GameObject prefab = null;

        foreach(var spawnprefab in spawnPrefabs)
        {
            if(spawnprefab.name == gameObjectName)
            {
                prefab = spawnprefab;

                _registerPrefabDictionary.Add(prefab.name, prefab);

                break;
            }
        }

        if(prefab == null)
        {
            return null;
        }

        return prefab;
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

        UIManager.Instance.DestoryUIManager();

        SceneManager.LoadScene("LoginScene");
    }

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        //ConnectionCount();
    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        //ConnectionCount();

        if (!Utils.IsSceneActive(GameplayScene))
        {
            return;
        }

        GameManager.Instance.RemoveLocalPlayer(conn.connectionId);
    }

    //private void ConnectionCount()
    //{
    //    var count = NetworkServer.connections.Count;

    //    NetworkUIManager.Instance.SetPlayerConnectionCount(count);
    //}

    /// <summary>
    /// 씬이 전환되면 호출되는 메서드.
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == "Assets/Scenes/ProjectScene.unity")
        {
            
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
