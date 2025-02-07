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

    private void SpawnPlayer()
    {
        var roomManager = Project_RoomManager.Instance;

        var spawnPositionComponent = FindFirstObjectByType<PlayerSpawnPosition>();

        if(spawnPositionComponent != null )
        {
            var position = spawnPositionComponent.GetSpawnTransform().position;

            var player = Instantiate(roomManager.spawnPrefabs[(int)SpawnEnumList.Player], position, Quaternion.identity);

            NetworkServer.Spawn(player, connectionToClient);

            LobbyPlayer lobbyplayerComponent = player.GetComponent<LobbyPlayer>();

            lobbyplayerComponent.SetMyPlayer();
        }
    }
}
