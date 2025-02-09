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

public enum UIEvent
{
    Health,
    MaxBullet,
    CurrentBullet
}

public enum PlayerLiveImage
{
    Live,
    Death
}

public enum DataKey
{
    Player,
    Enemy
}