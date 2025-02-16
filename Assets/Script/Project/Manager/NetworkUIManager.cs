using UnityEngine;
using Mirror;

public class NetworkUIManager : NetworkBehaviour
{
    private static NetworkUIManager _instance;
    public static NetworkUIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<NetworkUIManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (isServer)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void DestroyNetworkUIManager()
    {
        _instance = null;

        Destroy(gameObject);
    }

    public void OnReadyUI(bool isOn)
    {
        ClientRpc_OnReadyUI(isOn);
    }

    [ClientRpc]
    private void ClientRpc_OnReadyUI(bool isOn)
    {
        UIManager.Instance.OnReadyUI(isOn);
    }

    public void SetPlayerConnectionCount()
    {
        var count = NetworkServer.connections.Count;

        ClientRpc_PlayerConnectionCount(count);
    }

    [ClientRpc]
    private void ClientRpc_PlayerConnectionCount(int count)
    {
        UIManager.Instance.ConnectionCount(count);
    }
}
