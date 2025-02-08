using UnityEngine;
using Mirror;

public class Project_RoomPlayer : NetworkRoomPlayer
{
    public static Project_RoomPlayer MyPlayer { get; set; }

    public Player Player { get; set; }

    public override void Start()
    {
        base.Start();

        if (isServer)
        {
            SpawnPlayer();
        }
    }

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

        var playerData = DataManager.Instance.PlayerData;

        lobbyplayerComponent.SetPlayerData(playerData);

        lobbyplayerComponent.SetMyPlayer();
    }
    #endregion
}
