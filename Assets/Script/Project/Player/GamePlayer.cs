using UnityEngine;
using Mirror;
using System.Collections;

public class GamePlayer : Player, ITakeDamaged
{
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

    public override void OnStopServer()
    {
        NetworkUIManager.Instance.UnregisterParty(netId);
    }

    [Command]
    private void CommandMakePartyUI(string name)
    {
        NetworkUIManager.Instance.RegisterParty(netId, name);
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

        CommandMakePartyUI(myPlayer.SyncNickName);
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
        gameObject.layer = LayerMask.NameToLayer("DeathPlayer");

        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        GameManager.Instance.SetDeathPlayer(identity);
    }

    [ClientRpc]
    public void ClientRpc_RespawnPlayer()
    {
        GamePlayerControl(true);
    }


    public void GamePlayerControl(bool enabled)
    {
        _animator.SetBool(_animationDie, !enabled);

        _playerInput.enabled = enabled;

        var gameUI = UIManager.Instance.GameUI;

        gameUI.DeathUI.SetActive(!enabled);
    }
    
}
