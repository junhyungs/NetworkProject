using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private Animator _animator;

    [SyncVar]
    private float _moveSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        
    }
}
