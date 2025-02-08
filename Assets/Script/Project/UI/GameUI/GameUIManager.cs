using System;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : Singleton<GameUIManager>
{
    private Dictionary<UIEvent, Delegate> _uiEventDictionary = new Dictionary<UIEvent, Delegate>();

    #region Client
    public void OnClickExitGame()
    {
        var roomManager = Project_RoomManager.Instance;

        roomManager.StopGame();
    }

    public void RegisterPlayerUIEvent<T>(UIEvent key, Action<T> action)
    {
        if (!_uiEventDictionary.ContainsKey(key))
        {
            _uiEventDictionary.Add(key, action);
        }
    }

    public void UnRegisterPlayerUIEvent<T>(UIEvent key, Action<T> action)
    {
        if (_uiEventDictionary.ContainsKey(key))
        {
            _uiEventDictionary[key] = Delegate.Remove(_uiEventDictionary[key], action);

            _uiEventDictionary[key] = null;
        }
    }

    public void TriggerPlayerUIEvent<T>(UIEvent key, T value)
    {
        if (_uiEventDictionary.TryGetValue(key, out Delegate callBack))
        {
            (callBack as Action<T>).Invoke(value);
        }
    }
    #endregion
}
