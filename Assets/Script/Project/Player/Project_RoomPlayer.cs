using UnityEngine;
using Mirror;

public class Project_RoomPlayer : NetworkRoomPlayer
{
    public override void Start()
    {
        base.Start();

        if (isServer)
        {

        }
    }

    private void SpawnPlayer()
    {

    }
}
