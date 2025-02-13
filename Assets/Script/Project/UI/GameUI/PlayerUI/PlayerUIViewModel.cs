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
        UIManager.Instance.RegisterUIEvent<float>(UIEvent.Health, SetPlayerHealth);
        UIManager.Instance.RegisterUIEvent<int>(UIEvent.MaxBullet, SetPlayerMaxBullet);
        UIManager.Instance.RegisterUIEvent<int>(UIEvent.CurrentBullet, SetPlayerCurrentBullet);
        UIManager.Instance.RegisterUIEvent<string>(UIEvent.NickName, SetPlayerNickName);
        UIManager.Instance.RegisterUIEvent<float>(UIEvent.Speed, SetPlayerSpeed);
        UIManager.Instance.RegisterUIEvent<float>(UIEvent.Damage, SetPlayerDamage);
    }

    public void UnregisterChangedEventOnDisable()
    {
        UIManager.Instance.UnRegisterUIEvent<float>(UIEvent.Health, SetPlayerHealth);
        UIManager.Instance.UnRegisterUIEvent<int>(UIEvent.MaxBullet, SetPlayerMaxBullet);
        UIManager.Instance.UnRegisterUIEvent<int>(UIEvent.CurrentBullet, SetPlayerCurrentBullet);
        UIManager.Instance.UnRegisterUIEvent<string>(UIEvent.NickName, SetPlayerNickName);
        UIManager.Instance.UnRegisterUIEvent<float>(UIEvent.Speed, SetPlayerSpeed);
        UIManager.Instance.UnRegisterUIEvent<float>(UIEvent.Damage, SetPlayerDamage);
    }
}

