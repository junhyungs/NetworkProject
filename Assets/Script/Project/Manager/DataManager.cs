using UnityEngine;
using System.Collections;

public class DataManager : NetworkSingleton<DataManager>
{
    private const string _playerDataPath = "Data/PlayerData";
    private const string _enemyDataPath = "Data/EnemyData";

    public PlayerData PlayerData { get; private set; }
    public EnemyData EnemyData { get; private set; }

    public override void OnStartServer()
    {
        if (isServer)
        {
            SetGameData();
        }
    }

    public void SetGameData()
    {
        StartCoroutine(LoadPlayerData());
        StartCoroutine(LoadEnemyData());
    }

    private IEnumerator LoadPlayerData()
    {
        var request = Resources.LoadAsync<ScriptableObject>(_playerDataPath);

        yield return new WaitUntil(() => { return request.isDone; });

        var scriptableObject = request.asset as ScriptableObject;

        if(scriptableObject is  PlayerData data)
        {
            PlayerData = data;
        }
    }

    private IEnumerator LoadEnemyData()
    {
        var request = Resources.LoadAsync<ScriptableObject>(_enemyDataPath);

        yield return new WaitUntil(() => { return request.isDone; });

        var scriptableObject = request.asset as ScriptableObject;

        if (scriptableObject is EnemyData data)
        {
            EnemyData = data;
        }
    }
}
