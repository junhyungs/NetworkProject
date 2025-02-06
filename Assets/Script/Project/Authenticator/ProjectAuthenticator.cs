using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;


public class ProjectAuthenticator : NetworkAuthenticator
{
    private readonly HashSet<NetworkConnection> _connectionSet = new HashSet<NetworkConnection>();

    #region Server
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<RequestAuthenticatorMessage>(RequestMessage, false);
    }
    public override void OnStopServer()
    {
       NetworkServer.UnregisterHandler<RequestAuthenticatorMessage>();
    }

    private void RequestMessage(NetworkConnectionToClient connection, RequestAuthenticatorMessage message)
    {
        if(!_connectionSet.Add(connection))
        {
            return;
        }

        bool isAuthenticator = message._authenticatorCode == AuthenticatorCode.Success;

        var receiveMessage = isAuthenticator ?
            new ReceiveAuthenticatorMessage()
            {
                _code = AuthenticatorCode.Success,
                _message = "Success!!"
            }
            :
            new ReceiveAuthenticatorMessage()
            {
                _code = AuthenticatorCode.Fail,
                _message = "Fail"
            };

        connection.Send(receiveMessage);

        if (isAuthenticator)
        {
            connection.authenticationData = message._nickName;

            ServerAccept(connection);
        }
        else
        {
            StartCoroutine(RejectClient(connection, 1f));
        }
    }

    private IEnumerator RejectClient(NetworkConnectionToClient connection, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ServerReject(connection);
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<ReceiveAuthenticatorMessage>(ReceiveMessage, false);
    }

    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<ReceiveAuthenticatorMessage>();
    }

    private void ReceiveMessage(ReceiveAuthenticatorMessage message)
    {
        if(message._code == AuthenticatorCode.Success)
        {
            ClientAccept();
        }
        else
        {
            Project_RoomManager.Instance.StopHost();

            Debug.Log("인증실패");
        }
    }

    public override void OnClientAuthenticate()
    {
        var message = new RequestAuthenticatorMessage()
        {
            _authenticatorCode = PlayerInformation.AuthenticatorCode,
            _nickName = PlayerInformation.NickName
        };

        NetworkClient.Send(message);
    }

    //private event Func<ReceiveAuthenticatorMessage> _receiveAuthenticatorEvent;
    //ReceiveAuthenticatorMessage result = _receiveAuthenticatorEvent.Invoke(); --> Func<T> 사용

    #endregion
}
