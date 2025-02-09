using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Project_RoomManager : NetworkRoomManager
{
    private Dictionary<DataKey, ScriptableObject> _dataDictionary = new Dictionary<DataKey, ScriptableObject>();    
    private readonly object _lock = new object();

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

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);

        ConnectionCount();
    }
}
