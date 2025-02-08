using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : MonoBehaviour
{
    [Header("PlayerHP")]
    [SerializeField] private Image _hpFillImage;

    [Header("BulletText")]
    [SerializeField] private Text _bulletText;

    [Header("PlayerImage")]
    [SerializeField] private Image _playerImage;

    [Header("PlayerSprite")]
    [SerializeField] private Sprite[] _playerSprites;

    private PlayerUIViewModel _viewModel;

    private void OnEnable()
    {
        if(_viewModel == null)
        {
            _viewModel = new PlayerUIViewModel();

            _viewModel.RegisterChangedEventOnEnable();

            _viewModel.PropertyChanged += OnPropertyChangedEvent;
        }
    }

    private void OnDisable()
    {
        _viewModel.UnregisterChangedEventOnDisable();

        _viewModel.PropertyChanged -= OnPropertyChangedEvent;
    }

    private void OnPropertyChangedEvent(object sender, PropertyChangedEventArgs eventArgs)
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
        }
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
