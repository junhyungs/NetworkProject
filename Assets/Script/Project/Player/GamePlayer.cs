using UnityEngine;
using Mirror;
using Unity.VisualScripting;

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
        }
    }

    private void SetMyPlayer()
    {
        var myPlayer = Project_RoomPlayer.MyPlayer;

        myPlayer.Player = this;

        GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.NickName, myPlayer.SyncNickName);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isItem = other.gameObject.layer == LayerMask.NameToLayer("Item");
        
        if (isItem)
        {
            PickUpItem(other);
        }
    }

    private void PickUpItem(Collider other)
    {
        if(other.TryGetComponent(out NetworkIdentity identity))
        {
            CommandPickUpItem(identity);
        }
    }

    #region Server
  
    #endregion

    #region Command
    [Command]
    private void CommandPickUpItem(NetworkIdentity itemIdentity)
    {
        if(itemIdentity.TryGetComponent(out Item item))
        {
            item.UseItem(this);

            item.ReturnItem();
        }
    }
    #endregion

    #region ClientRPC

    #endregion
}
