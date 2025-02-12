using Mirror;
using UnityEngine;
using System;
using System.Collections;

public class Item : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemData _itemData;

    [Header("RotateSpeed")]
    [SerializeField] private float _rotateSpeed;

    private SphereCollider _sphereCollider;
    private Action<int> _returnCallBack;
    private Coroutine _returnDelayCoroutine;
    private Vector3 _rotateDirection = Vector3.up;
    private int _currentIndex;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _sphereCollider.enabled = true;
    }

    private void Update()
    {
        RotateItem();
    }

    private void RotateItem()
    {
        transform.Rotate(_rotateDirection * _rotateSpeed * Time.deltaTime, Space.World);

        //transform.rotation *= Quaternion.AngleAxis(_rotateSpeed * Time.deltaTime, _rotateDirection);
    }

    public void UseItem(GamePlayer player)
    {
        if(_returnDelayCoroutine != null)
        {
            StopCoroutine(_returnDelayCoroutine);

            _returnDelayCoroutine = null;
        }
      
        _sphereCollider.enabled = false;

        _itemData.UseItem(player);
    }

    [Server]
    public void StartReturnDelayCoroutine()
    {
        _returnDelayCoroutine = StartCoroutine(ReturnDelay());
    }

    [Server]
    public IEnumerator ReturnDelay()
    {
        yield return new WaitForSeconds(20f);

        _returnDelayCoroutine = null;

        ReturnItem();
    }

    [Server]
    public void ReturnItem()
    {
        _returnCallBack.Invoke(_currentIndex);

        NetworkServer.UnSpawn(gameObject);

        var objectPool = ObjectPool.Instance;

        objectPool.EnqueuePool(gameObject);
    }

    public void SetEvent(Action<int> callBack, int index)
    {
        _currentIndex = index;
        _returnCallBack = callBack;
    }
}
