using System.ComponentModel;
using UnityEngine;

public class PlayerUIViewModel
{
    private float _playerHealth;
    private int _maxBullet;
    private int _currentBullet;

    public float PlayerHealth
    {
        get => _playerHealth;
        set
        {
            if(_playerHealth == value)
            {
                return;
            }

            _playerHealth = value;

            OnPropertyChanged(nameof(PlayerHealth));
        }
    }

    public int MaxBullet
    {
        get => _maxBullet;
        set
        {
            if(_maxBullet == value)
            {
                return;
            }

            _maxBullet = value;

            OnPropertyChanged(nameof(MaxBullet));
        }
    }

    public int CurrentBullet
    {
        get => _currentBullet;
        set
        {
            if(_currentBullet == value)
            {
                return;
            }

            _currentBullet = value;

            OnPropertyChanged(nameof(CurrentBullet));
        }
    }

    public void SetPlayerHealth(float health)
    {
        PlayerHealth = health;
    }

    public void SetPlayerMaxBullet(int maxBullet)
    {
        MaxBullet = maxBullet;
    }

    public void SetPlayerCurrentBullet(int currentBullet)
    {
        CurrentBullet = currentBullet;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void RegisterChangedEventOnEnable()
    {
        GameUIManager.Instance.RegisterPlayerUIEvent<float>(UIEvent.Health, SetPlayerHealth);
        GameUIManager.Instance.RegisterPlayerUIEvent<int>(UIEvent.MaxBullet, SetPlayerMaxBullet);
        GameUIManager.Instance.RegisterPlayerUIEvent<int>(UIEvent.CurrentBullet, SetPlayerCurrentBullet);
    }

    public void UnregisterChangedEventOnDisable()
    {
        GameUIManager.Instance.UnRegisterPlayerUIEvent<float>(UIEvent.Health, SetPlayerHealth);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<int>(UIEvent.MaxBullet, SetPlayerMaxBullet);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<int>(UIEvent.CurrentBullet, SetPlayerCurrentBullet);
    }
}

