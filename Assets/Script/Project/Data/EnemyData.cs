using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _health;

    public float MoveSpeed => _moveSpeed;
    public float Damage => _damage;
    public float Health => _health;
}