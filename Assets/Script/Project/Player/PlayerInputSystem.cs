using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [Header("MoveVector")]
    [SerializeField] private Vector2 _moveVector;

    [Header("Delta")]
    [SerializeField] private float _mouseDelta;

    private bool _isZoom;
    private bool _isAttack;

    public Vector2 MoveVector => _moveVector;
    public float MouseDelta
    {
        get => _mouseDelta;
        set => _mouseDelta = value;
    }

    private event Action<bool> _sprintAction;
    private event Action<bool> _attackAction;
    private event Action<bool> _reloadAction;
    private event Action<bool> _zoomAction;

    public void SetSprintAction(Action<bool> callBack, bool isRegister)
    {
        if (isRegister)
        {
            _sprintAction += callBack;
        }
        else
        {
            _sprintAction -= callBack;
        }
    }

    public void SetAttackAction(Action<bool> callBack, bool isRegister)
    {
        if (isRegister)
        {
            _attackAction += callBack;
        }
        else
        {
            _attackAction -= callBack;
        }
    }

    public void SetReloadAction(Action<bool> callBack, bool isRegister)
    {
        if(isRegister)
        {
            _reloadAction += callBack;
        }
        else
        {
            _reloadAction -= callBack;
        }
    }

    public void SetZoomAction(Action<bool> callBack, bool isRegister)
    {
        if(isRegister)
        {
            _zoomAction += callBack;
        }
        else
        {
            _zoomAction -= callBack;
        }
    }

    public void OnMove(InputValue value)
    {
        if (_isZoom || _isAttack)
        {
            return;
        }

        SetMoveVector(value.Get<Vector2>());
    }

    private void SetMoveVector(Vector2 value)
    {
        _moveVector = value;
    }

    public void OnMouse(InputValue value)
    {
        SetDelta(value.Get<float>());
    }

    private void SetDelta(float delta)
    {
        _mouseDelta = delta;
    }

    public void OnAttack(InputValue value)
    {
        bool isPressed = value.isPressed;
        
        _isAttack = isPressed;

        _attackAction.Invoke(isPressed);
    }

    public void OnSprint(InputValue value)
    {
        bool isPressed = value.isPressed;

        _sprintAction.Invoke(isPressed);
    }

    public void OnReload(InputValue value)
    {
        bool isPressed = value.isPressed;

        _reloadAction.Invoke(isPressed);
    }

    public void OnZoom(InputValue value)
    {
        bool isPressed = value.isPressed;

        if (isPressed)
        {
            _moveVector = Vector2.zero;
        }

        _isZoom = isPressed;

        _zoomAction.Invoke(isPressed);
    }
}
