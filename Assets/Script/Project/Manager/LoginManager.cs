using UnityEngine;
using System;
public class LoginManager
{
    private static LoginManager instance;

    public static LoginManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new LoginManager();
            }

            return instance;
        }
    }

    public void OnClickLogin(string userId, string userPassword,
        Action<ReceiveAuthenticatorMessage> callBack)
    {
        LoginAuthenticator(userId, userPassword, callBack);
    }

    public void OnClickSignUp(string userId, string userPassword,
        Action<ReceiveSignUpMessage> callBack)
    {
        SignUp(userId, userPassword, callBack);
    }

    public void OnClickDuplicateCheck(string userId, string userPassword,
        Action<ReceiveDuplicateMessage> callBack)
    {
        Duplicate(userId, userPassword, callBack);  
    }

    private void LoginAuthenticator(string userId, string userpassword,
        Action<ReceiveAuthenticatorMessage> callBack)
    {
        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isLoginSuccess = false;

        if (isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _userId = userId,
                _userPassword = userpassword
            };

            var receiveDBMessage = DataBaseManager.Instance.Login(requestDBMessage);
            isLoginSuccess = receiveDBMessage._code == DB_MessageCode.LoginSuccess;
        }

        var receiveMessage = CreateReceiveAuthenticatorMessage(isLoginSuccess);

        if(receiveMessage._code == AuthenticatorCode.Success)
        {
            PlayerInformation.AuthenticatorCode = receiveMessage._code;
        }

        callBack.Invoke(receiveMessage);
    }

    private void SignUp(string userId, string userPassword,
        Action<ReceiveSignUpMessage> callBack)
    {
        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isInserted = false;

        if (isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _userId = userId,
                _userPassword = userPassword
            };

            var receiveDBMessage = DataBaseManager.Instance.INSERT_ID(requestDBMessage);
            isInserted = receiveDBMessage._code == DB_MessageCode.INSERT_Success;
        }

        var receiveMessage = CreateReceiveSignUpMessage(isInserted);

        callBack.Invoke(receiveMessage);
    }

    private void Duplicate(string userId, string userPassword,
        Action<ReceiveDuplicateMessage> callBack)
    {
        var DBConnectionMessage = DataBaseManager.Instance.ConnectionDataBase();

        bool isDBConnected = DBConnectionMessage._code == DB_MessageCode.ConnectionSuccess;
        bool isDuplicateCheck = false;

        if (isDBConnected)
        {
            var requestDBMessage = new RequestDBMessage()
            {
                _userId = userId,
                _userPassword = userPassword
            };

            var receiveDBMessage = DataBaseManager.Instance.DuplicateCheck(requestDBMessage);
            isDuplicateCheck = receiveDBMessage._code == DB_MessageCode.DuplicateSuccess;
        }

        var receiveMessage = CreateReceiveDuplicateMessage(isDuplicateCheck);

        callBack.Invoke(receiveMessage);
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
}

public class PlayerInformation
{
    public static AuthenticatorCode AuthenticatorCode;
    public static string NickName;
}

public class Data
{
    public string ID { get; set; }
    public string NickName { get; set; }
}