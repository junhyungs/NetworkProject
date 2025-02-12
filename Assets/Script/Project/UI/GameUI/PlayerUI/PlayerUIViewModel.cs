using System.ComponentModel;
using UnityEngine;

public class PlayerUIViewModel
{
    private float _playerHealth;
    private float _playerSpeed;
    private float _playerDamage;
    private int _maxBullet;
    private int _currentBullet;
    private string _nickName;

    public float PlayerSpeed
    {
        get => _playerSpeed;
        set
        {
            if(_playerSpeed == value)
            {
                return;
            }

            _playerSpeed = value;

            OnPropertyChanged(nameof(PlayerSpeed));
        }
    }

    public float PlayerDamage
    {
        get => _playerDamage;
        set
        {
            if(_playerDamage == value)
            {
                return;
            }

            _playerDamage = value;
            OnPropertyChanged(nameof(PlayerDamage));
        }
    }

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

    public string NickName
    {
        get => _nickName;
        set
        {
            if(_nickName == value)
            {
                return;
            }

            _nickName = value;

            OnPropertyChanged(nameof(NickName));
        }
    }

    public void SetPlayerSpeed(float speed)
    {
        PlayerSpeed = speed;
    }

    public void SetPlayerDamage(float damage)
    {
        PlayerDamage = damage;
    }

    public void SetPlayerNickName(string nickName)
    {
        NickName = nickName;
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
        GameUIManager.Instance.RegisterPlayerUIEvent<string>(UIEvent.NickName, SetPlayerNickName);
        GameUIManager.Instance.RegisterPlayerUIEvent<float>(UIEvent.Speed, SetPlayerSpeed);
        GameUIManager.Instance.RegisterPlayerUIEvent<float>(UIEvent.Damage, SetPlayerDamage);
    }

    public void UnregisterChangedEventOnDisable()
    {
        GameUIManager.Instance.UnRegisterPlayerUIEvent<float>(UIEvent.Health, SetPlayerHealth);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<int>(UIEvent.MaxBullet, SetPlayerMaxBullet);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<int>(UIEvent.CurrentBullet, SetPlayerCurrentBullet);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<string>(UIEvent.NickName, SetPlayerNickName);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<float>(UIEvent.Speed, SetPlayerSpeed);
        GameUIManager.Instance.UnRegisterPlayerUIEvent<float>(UIEvent.Damage, SetPlayerDamage);
    }
}

