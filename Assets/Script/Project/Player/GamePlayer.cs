using UnityEngine;
using Mirror;

public class GamePlayer : Player
{
    public PlayerData PlayerData { get; set; }

    protected override void Start()
    {
        base.Start();

        ChangePlayer();
    }

    private void ChangePlayer()
    {
        if (isOwned)
        {
            SetMyPlayer();

            CommandInitGamePlayerData();
        }
    }

    private void SetMyPlayer()
    {
        var myPlayer = Project_RoomPlayer.MyPlayer;

        myPlayer.Player = this;

        GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.NickName, myPlayer.SyncNickName);
    }

    #region Server
    [Server]
    public override void SetPlayerData(PlayerData data)
    {
        base.SetPlayerData(data);
    }
    #endregion

    #region Command
    [Command]
    private void CommandInitGamePlayerData()
    {
        SetPlayerData(PlayerData);
    }
    #endregion

    #region ClientRPC

    #endregion
}
