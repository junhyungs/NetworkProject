using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class LoginUIManager : Singleton<LoginUIManager> 
{
    [Header("OnlineUI")]
    [SerializeField] private GameObject _onlineUI;

    public void ActiveOnlineUI(bool isActive)
    {
        _onlineUI.SetActive(isActive);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Login(string id, string password,
        Action<ReceiveAuthenticatorMessage> callBack)
    {
        var loginManager = LoginManager.Instance;

        loginManager.OnClickLogin(id, password, callBack);
    }

    public void SignUp_CreateID(string id, string password, 
        Action<ReceiveSignUpMessage> callBack)
    {
        var loginManager = LoginManager.Instance;

        loginManager.OnClickSignUp(id, password, callBack);
    }

    public void SignUp_DuplicateCheck(string id, string password, 
        Action<ReceiveDuplicateMessage> callBack)
    {
        var loginManager = LoginManager.Instance;

        loginManager.OnClickDuplicateCheck(id, password, callBack);
    }
}
