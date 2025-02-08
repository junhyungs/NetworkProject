using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.SceneManagement;

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

    public void StopGame()
    {
        if(NetworkServer.active && NetworkClient.active)
        {
            singleton.StopHost();
        }
        else if(NetworkClient.active)
        {
            singleton.StopClient();
        }
        else if(NetworkServer.active)
        {
            singleton.StopServer();
        }

        SceneManager.LoadScene("LoginScene");
    }
}
