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
    private Vector3 _rotateDirection = Vector3.up;
    private int _currentIndex;

    public ObjectName CurrentName
    {
        get
        {
            var name = _itemData.ItemName;

            return name;
        }
    }

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
        _sphereCollider.enabled = false;

        _itemData.UseItem(player);
    }

    [Server]
    public IEnumerator ReturnDelay()
    {
        yield return new WaitForSeconds(20f);

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
