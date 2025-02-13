using UnityEngine;
using Mirror;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
            }

            return _instance;
        }
    }
}

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
            }

            return _instance;
        }
    }
}
