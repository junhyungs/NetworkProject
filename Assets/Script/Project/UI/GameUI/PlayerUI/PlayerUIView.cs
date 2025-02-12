using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : PlayerUI
{
    #region PlayerUI
    [Header("PlayerHP")]
    [SerializeField] private Image _hpFillImage;

    [Header("PlayerNickName")]
    [SerializeField] private Text _nickNameText;

    [Header("BulletText")]
    [SerializeField] private Text _bulletText;

    [Header("PlayerImage")]
    [SerializeField] private Image _playerImage;

    [Header("PlayerSprite")]
    [SerializeField] private Sprite[] _playerSprites;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        _viewModel.PropertyChanged += OnPropertyChangedEvent;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();

        _viewModel.PropertyChanged -= OnPropertyChangedEvent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnPropertyChangedEvent(object sender, PropertyChangedEventArgs eventArgs)
    {
        switch(eventArgs.PropertyName)
        {
            case nameof(_viewModel.PlayerHealth):
                PlayerHealthImage(_viewModel.PlayerHealth);
                break;
            case nameof(_viewModel.MaxBullet):
                SetPlayerBulletText();
                break;
            case nameof(_viewModel.CurrentBullet):
                SetPlayerBulletText();
                break;
            case nameof(_viewModel.NickName):
                SetPlayerNickName();
                break;
        }
    }

    private void SetPlayerNickName()
    {
        _nickNameText.text = _viewModel.NickName;
    }

    private void SetPlayerBulletText()
    {
        _bulletText.text = $"{_viewModel.CurrentBullet} / {_viewModel.MaxBullet}";
    }

    private void PlayerHealthImage(float value)
    {
        var fillValue = value / 100f;

        _hpFillImage.fillAmount = fillValue;

        SetPlayerImage(_hpFillImage.fillAmount > 0f);
    }

    private void SetPlayerImage(bool isAlive)
    {
        _playerImage.sprite = isAlive ? _playerSprites[(int)PlayerLiveImage.Live] :
            _playerSprites[(int)PlayerLiveImage.Death];
    }
}
