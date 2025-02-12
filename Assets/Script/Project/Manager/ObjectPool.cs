using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class Pool
{
    public Pool(Transform poolTransform)
    {
        GameObjectQueue = new Queue<GameObject>();
        PoolTransform = poolTransform;
        PoolCount = 0;
    }

    public Queue<GameObject> GameObjectQueue;
    public Transform PoolTransform;
    public GameObject SaveItem;
    public int PoolCount;

    public void EnqueuePool(GameObject gameObject)
    {
        gameObject.SetActive(false);

        GameObjectQueue.Enqueue(gameObject);
    }

    public GameObject DequeuePool()
    {
        var gameObject = GameObjectQueue.Dequeue();

        gameObject.SetActive(true);

        return gameObject;
    }
}

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<string, Pool> _objectPool = new Dictionary<string, Pool>();

    [Server]
    public bool CreatePool(GameObject item, int count = 20)
    {
        var newObjectName = item.name;

        if(!_objectPool.ContainsKey(newObjectName))
        {
            var childObject = new GameObject(newObjectName + "Pool");

            childObject.transform.SetParent(transform);

            var pool = new Pool(childObject.transform);

            pool.SaveItem = item;

            _objectPool.Add(newObjectName, pool);
        }

        CreateItem(item, _objectPool[newObjectName], newObjectName, count);

        return true;
    }

    private void CreateItem(GameObject item, Pool pool, string name, int count = 20)
    {
        for (int i = 0; i < count; i++)
        {
            var gameObject = Instantiate(item, pool.PoolTransform);

            gameObject.name = name;

            pool.EnqueuePool(gameObject);

            pool.PoolCount++;
        }
    }

    private bool IsPool(string objectName)
    {
        return _objectPool.ContainsKey(objectName);
    }

    [Server]
    public void EnqueuePool(GameObject item)
    {
        string name = item.name;

        if(!IsPool(name))
        {
            CreatePool(item);
        }

        item.transform.SetParent(_objectPool[name].PoolTransform);

        _objectPool[name].EnqueuePool(item);
    }

    [Server]
    public void AllDeactivePool(GameObject item)
    {
        string name = item.name;

        if (!IsPool(name))
        {
            return;
        }

        var pool = _objectPool[name];

        for(int i = 0; i < pool.PoolCount; i++)
        {
            GameObject poolObject = pool.PoolTransform.GetChild(i).gameObject;

            if (poolObject.activeSelf)
            {
                EnqueuePool(poolObject);
            }
        }
    }

    [Server]
    public GameObject DequeuePool(string objectName)
    {
        if (!IsPool(objectName))
        {
            return null;
        }

        var pool = _objectPool[(objectName)];

        if(pool.GameObjectQueue.Count <= 0)
        {
            GameObject saveItem = pool.SaveItem;

            CreateItem(saveItem, pool, objectName);
        }

        GameObject returnObject = pool.DequeuePool();

        returnObject.transform.parent = null;

        return returnObject;
    }
}
