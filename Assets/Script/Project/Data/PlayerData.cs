using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _health;
    [SerializeField] private int _maxBullet;

    public float MoveSpeed => _moveSpeed;
    public float Damage => _damage;
    public float Health => _health;
    public int MaxBullet => _maxBullet;
}
