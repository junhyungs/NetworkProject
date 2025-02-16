using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : Singleton<UIManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DestoryUIManager()
    {
        _instance = null;

        Destroy(gameObject);
    }

    #region GameReadyUI
    [Header("GameReadyUI")]
    [SerializeField] private GameReadyUI _gameReadyUI;
    public GameReadyUI GameReadyUI => _gameReadyUI;

    public void OnReadyUI(bool isOn)
    {
        if (isOn)
        {
            _gameReadyUI.OnReadyUI(isOn);
        }
        else
        {
            _gameReadyUI.OnReadyUI(isOn);

            _gameUI.PlayerPanel.SetActive(true);
        }
    }
    #endregion

    #region GameUI
    [Header("GameUI")]
    [SerializeField] private GameUI _gameUI;
    public GameUI GameUI => _gameUI;
    #endregion

    #region PlayerCountUI
    [Header("PlayerCountUI")]
    [SerializeField] private PlayerCountUI _playerCountUI;
   
    public void ConnectionCount(int count)
    {
        _playerCountUI.PlayerCount(count);
    }

    #endregion

    #region MVVM
    private Dictionary<UIEvent, Delegate> _uiEventDictionary = new Dictionary<UIEvent, Delegate>();

    public void RegisterUIEvent<T>(UIEvent key, Action<T> action)
    {
        if (!_uiEventDictionary.ContainsKey(key))
        {
            _uiEventDictionary.Add(key, action);
        }
    }

    public void UnRegisterUIEvent<T>(UIEvent key, Action<T> action)
    {
        if(_uiEventDictionary.ContainsKey(key))
        {
            var currentDelegate = Delegate.Remove(_uiEventDictionary[key], action);
            //_uiEventDictionary[key] ��������Ʈ���� �Ű������� ���� Action<T>�� �����ϰ�,
            //_uiEventDictionary[key] ��������Ʈ�� �ٽ� ��ȯ.

            //���� ��������Ʈ�� null�� �ǰų�, ��������Ʈ�� ����� �޼��尡 0����� ��ųʸ����� ����.
            if (currentDelegate == null ||
                currentDelegate.GetInvocationList().Length == 0)
            {                                                   
                _uiEventDictionary.Remove(key);
            }
            else
            {
                //�ƴ϶�� �ٽ� _uiEventDictionary[key]�� ��������Ʈ�� �Ҵ�.
                _uiEventDictionary[key] = currentDelegate;
            }
        }
    }

    public void TriggerUIEvent<T>(UIEvent key, T value)
    {
        if (_uiEventDictionary.TryGetValue(key, out Delegate callBack))
        {
            (callBack as Action<T>).Invoke(value);  
        }
    }
    #endregion
}
