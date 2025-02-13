using UnityEngine;
using Mirror;

public class GamePlayer : Player, ITakeDamaged
{
    public PlayerData PlayerData { get; set; }  

    protected override void Start()
    {
        base.Start();

        ChangePlayer();

        var networkIdentity = GetComponent<NetworkIdentity>();

        if(networkIdentity != null)
        {
            CommandRegisterLocalPlayer(networkIdentity);
        }
    }

    [Command]
    private void CommandRegisterLocalPlayer(NetworkIdentity identity)
    {
        GameManager.Instance.AddLocalPlayer(connectionToClient.connectionId, identity);
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

        UIManager.Instance.TriggerUIEvent(UIEvent.NickName, myPlayer.SyncNickName);
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
        if(itemIdentity == null)
        {
            return;
        }

        if(itemIdentity.TryGetComponent(out Item item))
        {
            item.UseItem(this);

            item.ReturnItem();
        }
    }

    [Server]
    public void TakeDamaged(float damage)
    {
        SyncHealth -= damage;
    }
    #endregion

    #region ClientRPC

    #endregion
}
