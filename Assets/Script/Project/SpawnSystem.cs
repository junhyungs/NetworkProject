using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class SpawnSystem : NetworkBehaviour
{
    [Header("Item")]    
    [SerializeField] private GameObject[] _itemObjectArray;
    [SerializeField] private Transform[] _itemSpawnTransformArray;
    [SerializeField] private float _itemSpawnTime;
    private Vector3 _spawnPosition = new Vector3(0f, 1f, 0f);
    private HashSet<int> _randomIndexSet = new HashSet<int>();

    [Header("Enemy")]
    [SerializeField] private GameObject[] _enemyObjectArray;
    [SerializeField] private Transform[] _enemySpawnTransformArray;
    [SerializeField] private int _spawnCount;
    [SerializeField] private float _nextWaveCoolTime;

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(SpawnItems());
        }
    }

    #region SpawnItem
    private GameObject[] SpawnItemPrefabs()
    {
        GameObject[] registerPrefabs = new GameObject[_itemObjectArray.Length];

        var roomManager = Project_RoomManager.Instance;

        for (int i = 0; i< registerPrefabs.Length; i++)
        {
            var itemName = _itemObjectArray[i].name;

            GameObject prefab = roomManager.GetRegisterPrefab(itemName);

            if (prefab != null)
            {
                registerPrefabs[i] = prefab;
            }
        }

        return registerPrefabs; 
    }

    private IEnumerator CreateItems(ObjectPool poolInstance)
    {
        GameObject[] spawnPrefabs = SpawnItemPrefabs();

        for(int i = 0; i < spawnPrefabs.Length; i++)
        {
            yield return new WaitUntil(() => 
            {
                return poolInstance.CreatePool(spawnPrefabs[i], 10);
            });
        }
    }

    private IEnumerator SpawnItems()
    {
        if(_itemSpawnTime <= 0)
        {
            yield break;
        }

        var objectPool = ObjectPool.Instance;

        yield return StartCoroutine(CreateItems(objectPool));

        while (true)
        {
            yield return new WaitForSeconds(_itemSpawnTime);

            if(_randomIndexSet.Count >= _itemSpawnTransformArray.Length)
            {
                continue;
            }

            int randomTransformIndex = Random.Range(0, _itemSpawnTransformArray.Length);
            
            if (!_randomIndexSet.Contains(randomTransformIndex))
            {
                Transform randomTransform = _itemSpawnTransformArray[randomTransformIndex];

                int randomItemIndex = Random.Range(0, _itemObjectArray.Length);
                var randomItemName = _itemObjectArray[randomItemIndex].name;

                GameObject randomItem = objectPool.DequeuePool(randomItemName);
                randomItem.transform.position = randomTransform.position + _spawnPosition;

                NetworkServer.Spawn(randomItem);

                Item item = randomItem.GetComponent<Item>();
                item.SetEvent(UnSetItem, randomTransformIndex);
                item.StartReturnDelayCoroutine();

                _randomIndexSet.Add(randomTransformIndex);
            }
        }
    }

    public void UnSetItem(int index)
    {
        if (_randomIndexSet.Contains(index))
        {
            _randomIndexSet.Remove(index);
        }
    }
    #endregion
    #region SpawnEnemy
    #endregion
}
