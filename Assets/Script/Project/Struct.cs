using Mirror;

public struct RequestDBMessage
{
    public string _userId;
    public string _userPassword;
}

public struct ReceiveDBMessage
{
    public DB_MessageCode _code;
    public string _message;
}

public struct RequestAuthenticatorMessage : NetworkMessage
{
    public AuthenticatorCode _authenticatorCode;
    public string _nickName;
}

public struct ReceiveAuthenticatorMessage : NetworkMessage
{
    public AuthenticatorCode _code;
    public string _message;
}

public struct ReceiveSignUpMessage
{
    public AuthenticatorCode _code;
    public string _message;
}

public struct ReceiveDuplicateMessage
{
    public AuthenticatorCode _code;
    public string _message;
}