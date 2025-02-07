using UnityEngine;

public enum DB_MessageCode
{
    ConnectionSuccess = 1,
    ConnectionFail,
    Fail,
    LoginSuccess,
    LoginFail,
    INSERT_Success,
    INSERT_Fall,
    DuplicateSuccess,
    DuplicateFall,
}

public enum AuthenticatorCode
{
    Success,
    Fail
}

public enum SpawnEnumList
{
    Player,
    Zombie,
}