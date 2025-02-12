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
    HealthItem,
    MoveSpeedItem,
    PowerUpItem,
    MaxBulletUpItem
}


public enum UIEvent
{
    Health,
    MaxBullet,
    CurrentBullet,
    NickName
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

public enum ObjectName
{
    HealthItem,
    MoveSpeedItem,
    PowerUpItem,
    MaxBulletUpItem
}