using UnityEngine;

using Mirror;

public class GamePlayer : Player, ITakeDamaged
{
    public PlayerData PlayerData { get; set; }

    protected override void Start()
    {
        base.Start();

        ChangePlayer();

        if (isOwned)
        {
            var networkIdentity = GetComponent<NetworkIdentity>();

            if (networkIdentity != null)
            {
                CommandRegisterLocalPlayer(networkIdentity);
            }
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

        if(SyncHealth <= 0)
        {
            ClientRpc_PlayerDeath();
        }
    }

    [ClientRpc]
    private void ClientRpc_PlayerDeath()
    {
        if (isOwned)
        {
            Command_SetDeathPlayer();

            GamePlayerControl(false);
        }
    }

    [Command]
    private void Command_SetDeathPlayer()
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();

        GameManager.Instance.SetDeathPlayer(identity);
    }

    public void GamePlayerControl(bool death)
    {
        _animator.SetBool(_animationDie, !death);

        _playerInput.enabled = death;

        var gameUI = UIManager.Instance.GameUI;

        gameUI.DeathUI();
    }
}
