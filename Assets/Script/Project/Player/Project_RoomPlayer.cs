using UnityEngine;
using Mirror;

public class Project_RoomPlayer : NetworkRoomPlayer
{
    public static Project_RoomPlayer MyPlayer { get; set; }
    public Player Player { get; set; }

    [SyncVar]
    private string _myNickName;
    public string SyncNickName
    {
        get => _myNickName;
        set
        {
            if (isServer)
            {
                _myNickName = value;
            }
        }
    }

    public override void Start()
    {
        base.Start();

        if (isServer)
        {
            SpawnPlayer();
        }
    }

    #region Client
  
    #endregion

    #region Server
    [Server]
    private void SpawnPlayer()
    {
        var roomManager = Project_RoomManager.Instance;

        var spawnPositionComponent = FindFirstObjectByType<PlayerSpawnPosition>();

        if(spawnPositionComponent != null )
        {
            var position = spawnPositionComponent.GetSpawnTransform().position;

            var player = Instantiate(roomManager.spawnPrefabs[(int)SpawnEnumList.Player], position, Quaternion.identity);

            NetworkServer.Spawn(player, connectionToClient);

            SetLobbyPlayer(player);
        }
    }

    [Server]
    private void SetLobbyPlayer(GameObject player)
    {
        LobbyPlayer lobbyplayerComponent = player.GetComponent<LobbyPlayer>();

        var playerData = Project_RoomManager.Instance.GetData<PlayerData>(DataKey.Player);

        if(playerData != null)
        {
            lobbyplayerComponent.SetPlayerData(playerData);
        }
        
        lobbyplayerComponent.SetMyPlayer();   
    }

    public void ReadyToBegin(bool isReady)
    {
        CmdChangeReadyState(isReady);
    }

    [Command]
    public void CommandRoomPlayerReady(bool isReady)
    {
        var roomManager = Project_RoomManager.Instance;

        roomManager.SetReadyPlayerCount(isReady);
    }

    [Command]
    public void CommandSetNickName(string nickName)
    {
        SyncNickName = nickName;
    }
    #endregion
}
