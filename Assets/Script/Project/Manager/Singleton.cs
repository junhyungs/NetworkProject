using UnityEngine;
using Mirror;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if(_instance == null)
                {
                    GameObject newObject = new GameObject(typeof(T).Name);

                    _instance = newObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject newObject = new GameObject(typeof(T).Name);

                    _instance = newObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}
