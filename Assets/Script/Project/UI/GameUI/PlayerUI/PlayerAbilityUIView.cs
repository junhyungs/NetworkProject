using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.ComponentModel;

public class PlayerAbilityUIView : PlayerUI
{
    [Header("TabAction")]
    [SerializeField] private InputActionReference _tabAction;

    [Header("AbilityPanel")]
    [SerializeField] private GameObject _abilityPanel;

    [Header("ValueText")]
    [SerializeField] private Text _hpValueText;
    [SerializeField] private Text _damageValueText;
    [SerializeField] private Text _speedValueText;
    [SerializeField] private Text _maxBulletValueText;

    protected override void OnEnable()
    {
        base.OnEnable();

        _viewModel.PropertyChanged += OnPropertyChangedEvent;

        _tabAction.action.Enable();

        _tabAction.action.performed += TabAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _viewModel.PropertyChanged -= OnPropertyChangedEvent;

        _tabAction.action.performed -= TabAction;

        _tabAction.action.Disable();
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
                _hpValueText.text = _viewModel.PlayerHealth.ToString();
                break;
            case nameof(_viewModel.PlayerDamage):
                _damageValueText.text = _viewModel.PlayerDamage.ToString();
                break;
            case nameof(_viewModel.PlayerSpeed):
                _speedValueText.text = _viewModel.PlayerSpeed.ToString();
                break;
            case nameof(_viewModel.MaxBullet):
                _maxBulletValueText.text = _viewModel.MaxBullet.ToString();
                break;
        }
    }

    private void TabAction(InputAction.CallbackContext callbackContext)
    {
        if (_abilityPanel.activeSelf)
        {
            _abilityPanel.SetActive(false);
        }
        else
        {
            _abilityPanel.SetActive(true);
        }
    }
}
