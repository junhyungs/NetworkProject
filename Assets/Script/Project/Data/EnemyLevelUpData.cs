using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLevelUpData", menuName = "Scriptable Data/EnemyLevelUpData")]
public class EnemyLevelUpData : ScriptableObject
{
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;

    public float Health => _health;
    public float Damage => _damage;
    public float Speed => _speed;
}
