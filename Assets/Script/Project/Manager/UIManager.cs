using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : Singleton<UIManager>
{
    #region GameUI
    [Header("GameUI")]
    [SerializeField] private GameUI _gameUI;

    public PlayerCountUI PlayerCountUI
    {
        get
        {
            if(_gameUI != null)
            {
                return _gameUI.PlayerCountUI;
            }

            return null;
        }
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
