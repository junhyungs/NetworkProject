using Mirror;

public struct RequestDBMessage
{
    public NetworkConnection _connection;
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
    public string _userId;
    public string _userPassword;
}

public struct ReceiveAuthenticatorMessage : NetworkMessage
{
    public AuthenticatorCode _code;
    public string _message;
}

public struct RequestSignUpMessage : NetworkMessage
{
    public string _userId;
    public string _userPassword;
}

public struct ReceiveSignUpMessage : NetworkMessage
{
    public AuthenticatorCode _code;
    public string _message;
}

public struct RequestDuplicateMessage : NetworkMessage
{
    public string _userId;
    public string _userPassword;
}

public struct ReceiveDuplicateMessage : NetworkMessage
{
    public AuthenticatorCode _code;
    public string _message;
}