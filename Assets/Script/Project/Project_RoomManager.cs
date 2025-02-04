using UnityEngine;
using Mirror;

public class Project_RoomManager : NetworkRoomManager
{
    public static Project_RoomManager Instance
    {
        get
        {
            if(singleton != null)
            {
                var instance = singleton as Project_RoomManager;

                return instance;
            }

            var roomManager = FindFirstObjectByType<Project_RoomManager>();

            if(roomManager != null)
            {
                return roomManager;
            }

            throw new System.Exception("RoomManager is Null");
        }
    }

    public ProjectAuthenticator ProjectAuthenticator { get; set; }

    public override void Awake()
    {
        base.Awake();

        ProjectAuthenticator = GetComponent<ProjectAuthenticator>();
    }

    public void StopHostNetwork()
    {
        singleton.StopHost();
    }
}
