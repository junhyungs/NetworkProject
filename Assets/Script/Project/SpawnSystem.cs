using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class SpawnSystem : NetworkBehaviour
{
    public bool SpawnSystemReady { get; set; } = false;
    public bool WaveClear { get; set; } = false;

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(InitializeSpawnSystem());
        }
    }

    private IEnumerator InitializeSpawnSystem()
    {
        StartCoroutine(SpawnItems());
        StartCoroutine(CreateEnemyObject());

        yield return new WaitUntil(() =>
        {
            return _itemReady && _enemyReady;
        });

        SpawnSystemReady = true;
    }

    #region SpawnItem
    [Header("Item")]    
    [SerializeField] private GameObject[] _itemObjectArray;
    [SerializeField] private Transform[] _itemSpawnTransformArray;
    [SerializeField] private float _itemSpawnTime;
    private bool _itemReady;
    private Vector3 _spawnPosition = new Vector3(0f, 1f, 0f);
    private HashSet<int> _randomIndexSet = new HashSet<int>();

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

        _itemReady = true;
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
    [Header("Enemy")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private GameObject[] _enemyObjectArray;
    [SerializeField] private Transform[] _enemySpawnTransformArray;
    [SerializeField] private int _spawnCount;
    [SerializeField] private int _countDown;

    [Header("LevelUpData")]
    [SerializeField] private EnemyLevelUpData _levelUpData;
    private int _currentLevel = 1;
    private bool _enemyReady;

    private float _currentZombieHealth;
    private float _currentZombiePower;
    private float _currentZombieSpeed;

    private Coroutine _spawnCoroutine;
    private HashSet<Zombie> _spawnZombieSet = new HashSet<Zombie>();

    private GameObject[] SpawnEnemyPrefabs()
    {
        GameObject[] enemyPrefabs = new GameObject[_enemyObjectArray.Length];

        var roomManager = Project_RoomManager.Instance;

        for(int i = 0; i < enemyPrefabs.Length; i++)
        {
            var prefabName = _enemyObjectArray[i].name;

            var prefab = roomManager.GetRegisterPrefab(prefabName);

            if(prefab != null)
            {
                enemyPrefabs[i] = prefab;
            }
        }

        return enemyPrefabs;
    }

    private IEnumerator CreateEnemyObject()
    {
        var objectPool = ObjectPool.Instance;

        var enemyPrefabs = SpawnEnemyPrefabs();

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            yield return new WaitUntil(() =>
            {
                return objectPool.CreatePool(enemyPrefabs[i]);
            });
        }

        _enemyReady = true;

        InitializeZombieAbility();
    }

    private void InitializeZombieAbility()
    {
        _currentZombieHealth = _data.Health;
        _currentZombiePower = _data.Damage;
        _currentZombieSpeed = _data.MoveSpeed;
    }

    public void StartSpawnEnemy()
    {
        WaveClear = false;

        _spawnCoroutine = StartCoroutine(SpawnEnemy());
    }

    public void ReGameSpawnSystem()
    {
        _currentLevel = 1;

        InitializeZombieAbility();
    }

    public void StopSpawnEnemy()
    {
        if(_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);

            _spawnCoroutine = null;
        }

        var spawnZombieSetToList = _spawnZombieSet.ToList();

        foreach(var enemy in spawnZombieSetToList)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.ZombieStop();
            }
        }
    }

    private IEnumerator SpawnEnemy()
    {
        yield return StartCoroutine(UICoroutine(_countDown, _currentLevel));

        var objectPool = ObjectPool.Instance;

        for(int i = 0; i < _spawnCount; i++)
        {
            int randomTransform = Random.Range(0, _enemySpawnTransformArray.Length);
            int randomEnemyIndex = Random.Range(0, _enemyObjectArray.Length);

            string randomEnemyName = _enemyObjectArray[randomEnemyIndex].name;
            Transform spawnTransform = _enemySpawnTransformArray[randomTransform];

            GameObject enemyObject = objectPool.DequeuePool(randomEnemyName);
            enemyObject.transform.position = spawnTransform.position;

            Transform target = GameManager.Instance.GetRandomLocalPlayerTransform();

            Zombie zombieComponent = enemyObject.GetComponent<Zombie>();
            zombieComponent.SetZombieData(_currentZombieHealth, _currentZombiePower,
                _currentZombieSpeed);
            zombieComponent.Target = target;
            zombieComponent.UnRegisterCallBack(UnSetEnemy);

            NavMeshAgent agent = enemyObject.GetComponent<NavMeshAgent>();

            if (!agent.enabled)
            {
                agent.enabled = true;
            }

            _spawnZombieSet.Add(zombieComponent);
            NetworkServer.Spawn(enemyObject);

            zombieComponent.SettingZombie();

            yield return new WaitForSeconds(1f);
        }

        EnemyPowerUp();

        _currentLevel++;
    }

    private void EnemyPowerUp()
    {
        _currentZombieHealth += _levelUpData.Health;
        _currentZombiePower += _levelUpData.Damage;
        _currentZombieSpeed += _levelUpData.Speed;
    }

    private void UnSetEnemy(Zombie zombie)
    {
        if (_spawnZombieSet.Contains(zombie))
        {
            _spawnZombieSet.Remove(zombie);

            if (_spawnZombieSet.Count <= 0)
            {
                WaveClear = true;
            }
        }
    }
    #endregion

    #region LevelUI
    [Header("LevelUI")]
    [Header("CountDownUI")]
    [SerializeField] private GameObject _countDownObject;
    [SerializeField] private Text _countDownText;
    [Header("CurrentLevel")]
    [SerializeField] private GameObject _currentLevelObject;
    [SerializeField] private Text _currentLevelText;
    [SerializeField] private float _duractionTime;

    [Server]
    private IEnumerator UICoroutine(int count, int currentLevel)
    {
        yield return StartCoroutine(CountDown(count));

        StartCoroutine(CurrentLevel(currentLevel));
    }

    public IEnumerator CountDown(int count)
    {
        ClientRpc_ActiveCountDownUI(true);

        while (count > 0)
        {
            ClientRpc_CountDown(count);

            yield return new WaitForSeconds(1f);

            count--;
        }

        ClientRpc_ActiveCountDownUI(false);
    }

    public IEnumerator CurrentLevel(int currentLevel)
    {
        ClientRpc_SetText(currentLevel);

        ClientRpc_ActiveLevelUI(true);

        yield return StartCoroutine(SetAlpha(_currentLevelText.color, 1f));

        yield return StartCoroutine(SetAlpha(_currentLevelText.color, 0f));

        ClientRpc_ActiveLevelUI(false);
    }

    private IEnumerator SetAlpha(Color color, float targetAlpha)
    {
        float startAlpha = color.a;
        float elapsed = 0f;
        float duractionTime = 3f;

        while (elapsed < duractionTime)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duractionTime);
            ClientRpc_SetAlpha(color.a);
            yield return null;
        }

        color.a = targetAlpha;
        ClientRpc_SetAlpha(color.a);
    }

    [ClientRpc]
    private void ClientRpc_SetText(int currentLevel)
    {
        _currentLevelText.text = $"Level <color=#FF0000>{currentLevel}</color>";
    }

    [ClientRpc]
    private void ClientRpc_ActiveLevelUI(bool isActive)
    {
        _currentLevelObject.SetActive(isActive);
    }

    [ClientRpc]
    private void ClientRpc_SetAlpha(float alpha)
    {
        Color color = _currentLevelText.color;

        color.a = alpha;

        _currentLevelText.color = color;
    }

    [ClientRpc]
    private void ClientRpc_ActiveCountDownUI(bool isActive)
    {
        _countDownObject.SetActive(isActive);
    }

    [ClientRpc]
    private void ClientRpc_CountDown(int count)
    {
        _countDownText.text = $"다음 공격까지 <color=#FF0000>{count}</color>초";
    }
    #endregion
}
