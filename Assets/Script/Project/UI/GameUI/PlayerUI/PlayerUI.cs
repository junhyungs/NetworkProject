using System.ComponentModel;
using UnityEngine;

public abstract class PlayerUI : MonoBehaviour
{
    protected static PlayerUIViewModel _viewModel;

    protected virtual void OnEnable()
    {
        if(_viewModel == null)
        {
            _viewModel = new PlayerUIViewModel();

            _viewModel.RegisterChangedEventOnEnable();
        }
    }

    protected virtual void OnDisable()
    {
        if(_viewModel != null)
        {
            _viewModel.UnregisterChangedEventOnDisable();
        }
    }

    protected virtual void OnDestroy()
    {
        _viewModel = null;
    }

    protected abstract void OnPropertyChangedEvent(object sender, PropertyChangedEventArgs eventArgs);
}
