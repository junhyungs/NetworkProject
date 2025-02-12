using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerSpawnPosition : MonoBehaviour
{
    [Header("PlayerSpawnPosition")]
    [SerializeField] private Transform[] _spawnPositions;

    private List<Transform> _transformList;

    private void Awake()
    {
        SetList();
    }

    [Server]
    public Transform GetSpawnTransform()
    {
        if(_transformList.Count < 0)
        {
            SetList();
        }

        var random = Random.Range(0, _transformList.Count);

        var position = _transformList[random];

        _transformList.Remove(position);

        return position;
    }

    private void SetList()
    {
        _transformList = new List<Transform>(_spawnPositions);
    }
}
