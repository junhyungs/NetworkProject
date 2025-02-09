using UnityEngine;
using System.Collections;
using Mirror;

public class DataManager : NetworkBehaviour
{
    private const string _playerDataPath = "Data/PlayerData";

    private const string _enemyDataPath = "Data/EnemyData";

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

        var roomManager = Project_RoomManager.Instance;

        roomManager.AddData(DataKey.Player, scriptableObject);
    }

    private IEnumerator LoadEnemyData()
    {
        var request = Resources.LoadAsync<ScriptableObject>(_enemyDataPath);

        yield return new WaitUntil(() => { return request.isDone; });

        var scriptableObject = request.asset as ScriptableObject;

        var roomManager = Project_RoomManager.Instance;

        roomManager.AddData(DataKey.Enemy, scriptableObject);
    }
}
