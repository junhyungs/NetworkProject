using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class LobbyPlayer : Player
{
    protected override void Start()
    {
        base.Start();
    }

    [Server]
    public override void SetPlayerData(PlayerData data)
    {
        base.SetPlayerData(data);
    }

    [Server]
    public void SetMyPlayer()
    {
        var roomManager = Project_RoomManager.Instance;

        var roomSlots = roomManager.roomSlots;

        MyPlayer(roomSlots.ToList());
    }

    [TargetRpc]
    private void MyPlayer(List<NetworkRoomPlayer> roomList)
    {
        foreach(var networkPlayer in roomList)
        {
            if (networkPlayer.isOwned)
            {
                var myPlayer = networkPlayer as Project_RoomPlayer;

                Project_RoomPlayer.MyPlayer = myPlayer;

                myPlayer.Player = this;

                var nickName = PlayerInformation.NickName;

                myPlayer.CommandSetNickName(nickName);

                GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.NickName, nickName);

                break;
            }
        }
    }

}
