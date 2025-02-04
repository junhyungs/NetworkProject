using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;


public class ProjectAuthenticator : NetworkAuthenticator
{
    private readonly HashSet<NetworkConnection> _connectionSet = new HashSet<NetworkConnection>();

    private void Start()
    {
        ServerRegister();
        ClientRegister();
    }

    private void ServerRegister()
    {
        NetworkServer.RegisterHandler<RequestDuplicateMessage>(OnRequestNetworkMessage, false);
        NetworkServer.RegisterHandler<RequestSignUpMessage>(OnRequestNetworkMessage, false);
        NetworkServer.RegisterHandler<RequestAuthenticatorMessage>(OnRequestNetworkMessage, false);
    }

    private void ClientRegister()
    {
        NetworkClient.RegisterHandler<ReceiveAuthenticatorMessage>(ServerToClient_AuthenticatorMessage, false);
        NetworkClient.RegisterHandler<ReceiveSignUpMessage>(ServerToClient_SignUpMessage, false);
        NetworkClient.RegisterHandler<ReceiveDuplicateMessage>(ServerToClient_DuplicateMessage, false);
    }

    #region Server
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<RequestAuthenticatorMessage>();
        NetworkServer.UnregisterHandler<RequestSignUpMessage>();
        NetworkServer.UnregisterHandler<RequestDuplicateMessage>();
    }

    #region T
    //private void RegisterNetworkServer<T>(Action<NetworkConnectionToClient, T> action, bool requireAuthentication)
    //    where T : struct, NetworkMessage
    //{
    //    NetworkServer.RegisterHandler(action, requireAuthentication);
    //}

    //private void UnRegister<T>() where T : struct, NetworkMessage
    //{
    //    NetworkServer.UnregisterHandler<T>();
    //}
    #endregion

    /// <summary>
    /// SendToClientMessage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="message"></param>
    private void OnRequestNetworkMessage<T>(NetworkConnectionToClient connection, T message)
    {
        if(typeof(T) == typeof(RequestAuthenticatorMessage))
        {
            AuthenticatorMessage(connection, (RequestAuthenticatorMessage)(object)message);
        }
        else if(typeof(T) == typeof(RequestSignUpMessage))
        {
            SignUpMessage(connection, (RequestSignUpMessage)(object)message);
        }
        else if(typeof(T) == typeof(RequestDuplicateMessage))
        {
            DuplicateMessage(connection, (RequestDuplicateMessage)(object)message);
        }
    }

    private void AuthenticatorMessage(NetworkConnectionToClient connection, RequestAuthenticatorMessage data)
    {
        if (!_connectionSet.Add(connection))
        {
            return;
        }

        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isLoginSuccess = false;

        if (isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _connection = connection,
                _userId = data._userId,
                _userPassword = data._userPassword,
            };

            var receiveDBMessage = DataBaseManager.Instance.Login(requestDBMessage);
            isLoginSuccess = receiveDBMessage._code == DB_MessageCode.LoginSuccess;
        }

        var receiveMessage = CreateReceiveAuthenticatorMessage(isLoginSuccess);

        connection.Send(receiveMessage);

        if (isLoginSuccess)
        {
            connection.authenticationData = data._userId;

            ServerAccept(connection);
        }
        else
        {
            StartCoroutine(RejectClient(connection, 1f));
        }

        _connectionSet.Remove(connection);
    }

    private void SignUpMessage(NetworkConnectionToClient connection, RequestSignUpMessage data)
    {
        if(!_connectionSet.Add(connection))
        {
            return;
        }

        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isInserted = false;

        if (isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _connection = connection,
                _userId = data._userId,
                _userPassword = data._userPassword,
            };

            var receiveDBMessage = DataBaseManager.Instance.INSERT_ID(requestDBMessage);
            isInserted = receiveDBMessage._code == DB_MessageCode.INSERT_Success;
        }

        var receiveMessage = CreateReceiveSignUpMessage(isInserted);

        connection.Send(receiveMessage);

        StartCoroutine(RejectClient(connection, 1f));

        _connectionSet.Remove(connection);
    }

    private void DuplicateMessage(NetworkConnectionToClient connection, RequestDuplicateMessage data)
    {
        if (!_connectionSet.Add(connection))
        {
            return;
        }

        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isDuplicateCheck = false;

        if(isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _connection = connection,
                _userId = data._userId,
                _userPassword = data._userPassword
            };

            var receiveDBMessage = DataBaseManager.Instance.DuplicateCheck(requestDBMessage);
            isDuplicateCheck = receiveDBMessage._code == DB_MessageCode.DuplicateSuccess;
        }

        var receiveMessage = CreateReceiveDuplicateMessage(isDuplicateCheck);

        connection.Send(receiveMessage);

        StartCoroutine(RejectClient(connection, 1f));

        _connectionSet.Remove(connection);
    }

    private IEnumerator RejectClient(NetworkConnectionToClient connection, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ServerReject(connection);
    }

    private ReceiveAuthenticatorMessage CreateReceiveAuthenticatorMessage(bool isSuccess)
    {
        return isSuccess ? new ReceiveAuthenticatorMessage()
        {
            _code = AuthenticatorCode.Success,
            _message = "Success"
        } :
        new ReceiveAuthenticatorMessage()
        {
            _code = AuthenticatorCode.Fail,
            _message = "Fail"
        };
    }

    private ReceiveSignUpMessage CreateReceiveSignUpMessage(bool isSuccess)
    {
        return isSuccess ? new ReceiveSignUpMessage()
        {
            _code = AuthenticatorCode.Success,
            _message = "Success"
        } :
        new ReceiveSignUpMessage()
        {
            _code = AuthenticatorCode.Fail,
            _message = "Fail"
        };
    }

    private ReceiveDuplicateMessage CreateReceiveDuplicateMessage(bool isSuccess)
    {
        return isSuccess ? new ReceiveDuplicateMessage()
        {
            _code = AuthenticatorCode.Success,
            _message = "Success"
        } :
        new ReceiveDuplicateMessage()
        {
            _code = AuthenticatorCode.Fail,
            _message = "Fail"
        };
    }
    #endregion

    #region Client

    //private event Func<ReceiveAuthenticatorMessage> _receiveAuthenticatorEvent;
    //ReceiveAuthenticatorMessage result = _receiveAuthenticatorEvent.Invoke(); --> Func<T> »ç¿ë

    private event Action<ReceiveAuthenticatorMessage> _receiveAuthenticatorEvent;
    private event Action<ReceiveSignUpMessage> _receiveSignUpEvent;
    private event Action<ReceiveDuplicateMessage> _receiveDuplicateEvent;
    
    private Dictionary<Delegate, Delegate> _delegateDictionary = new Dictionary<Delegate, Delegate>();

    public override void OnStopClient()
    {
        UnRegisterActionEvent(_receiveAuthenticatorEvent);
        UnRegisterActionEvent(_receiveSignUpEvent);
        UnRegisterActionEvent(_receiveDuplicateEvent);

        NetworkClient.UnregisterHandler<ReceiveAuthenticatorMessage>();
        NetworkClient.UnregisterHandler<ReceiveSignUpMessage>();
        NetworkClient.UnregisterHandler<ReceiveDuplicateMessage>();
    }

    
    public void OnClickLogin(string userId, string userPassword, Action<ReceiveAuthenticatorMessage> callBack)
    {
        if(_receiveAuthenticatorEvent == null &&
            !_delegateDictionary.ContainsKey(_receiveAuthenticatorEvent))
        {
            _receiveAuthenticatorEvent += callBack;
            _delegateDictionary.Add(_receiveAuthenticatorEvent, callBack);
        }

        var message = CreateRequestAuthenticatorMessage(userId, userPassword);

        NetworkClient.Send(message);
    }

    private void ServerToClient_AuthenticatorMessage(ReceiveAuthenticatorMessage message)
    {
        if(_receiveAuthenticatorEvent != null)
        {
            _receiveAuthenticatorEvent.Invoke(message);
        }
    }

    public void OnClickSignUp(string userId, string userPassword, Action<ReceiveSignUpMessage> callBack)
    {
        if(_receiveSignUpEvent == null &&
            !_delegateDictionary.ContainsKey(_receiveSignUpEvent))
        {
            _receiveSignUpEvent += callBack;
            _delegateDictionary.Add(_receiveSignUpEvent, callBack);
        }

        var message = CreateRequestSignUpMessage(userId, userPassword);

        NetworkClient.Send(message);
    }

    private void ServerToClient_SignUpMessage(ReceiveSignUpMessage message)
    {
        if(_receiveSignUpEvent != null)
        {
            _receiveSignUpEvent.Invoke(message);
        }
    }

    public void OnClickDuplicateCheck(string userId, string userPassword, Action<ReceiveDuplicateMessage> callBack)
    {
        if(_receiveDuplicateEvent == null &&
            !_delegateDictionary.ContainsKey(_receiveDuplicateEvent))
        {
            _receiveDuplicateEvent += callBack;
            _delegateDictionary.Add(_receiveDuplicateEvent, callBack);
        }

        var message = CreateRequestDuplicateMessage(userId, userPassword);

        NetworkClient.Send(message);
    }

    private void ServerToClient_DuplicateMessage(ReceiveDuplicateMessage message)
    {
        if(_receiveDuplicateEvent != null)
        {
            _receiveDuplicateEvent.Invoke(message);
        }
    }

    private RequestAuthenticatorMessage CreateRequestAuthenticatorMessage(string userId, string userPassword)
    {
        return new RequestAuthenticatorMessage()
        {
            _userId = userId,
            _userPassword = userPassword
        };
    }

    private RequestSignUpMessage CreateRequestSignUpMessage(string userId, string userPassword)
    {
        return new RequestSignUpMessage()
        {
            _userId = userId,
            _userPassword = userPassword
        };
    }

    private RequestDuplicateMessage CreateRequestDuplicateMessage(string userId, string userPassword)
    {
        return new RequestDuplicateMessage()
        {
            _userId = userId,
            _userPassword = userPassword
        };
    }

    private void UnRegisterActionEvent<T>(Action<T> key)
    {
        if (_delegateDictionary.ContainsKey(key))
        {
            _delegateDictionary.Remove(key);
        }

        if (typeof(T) == typeof(ReceiveAuthenticatorMessage))
        {
            _receiveAuthenticatorEvent = null;
        }
        else if (typeof(T) == typeof(ReceiveSignUpMessage))
        {
            _receiveSignUpEvent = null;
        }
        else if (typeof(T) == typeof(ReceiveDuplicateMessage))
        {
            _receiveDuplicateEvent = null;
        }
    }
    #endregion
}
