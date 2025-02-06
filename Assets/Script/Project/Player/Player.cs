using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private PlayerInputSystem _inputSystem;
    private PlayerCamera _playerCamera;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Camera _camera;

    private readonly int _animationMovement = Animator.StringToHash("SpeedValue");
    private readonly int _animationFire = Animator.StringToHash("Fire");
    private readonly int _animationReload = Animator.StringToHash("Reloading");

    private float _currentMoveSpeed = 3f;
    private float _targetSpeed;
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;

    private bool _isZoomMode;

    private void Awake()
    {
        InitializeOnAwakePlayer();
    }

    private void InitializeOnAwakePlayer()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputSystem = GetComponent<PlayerInputSystem>();
        _animator = GetComponent<Animator>();
        _animator.applyRootMotion = true;
        _playerCamera = GetComponentInChildren<PlayerCamera>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        OnEnablePlayer();
    }

    private void OnEnablePlayer()
    {
        SettingAction(true);
    }

    private void OnDisable()
    {
        OnDisablePlayer();  
    }

    private void OnDisablePlayer()
    {
        SettingAction(false);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_isZoomMode)
        {
            float mouseDelta = _inputSystem.MouseDelta;

            Quaternion rotation = Quaternion.Euler(0f, mouseDelta, 0f);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3f * Time.fixedDeltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        _targetSpeed = _currentMoveSpeed;

        if(_inputSystem.MoveVector == Vector2.zero)
        {
            _targetSpeed = 0f;
        }

        float animationSpeed = _animator.deltaPosition.magnitude / Time.fixedDeltaTime;

        bool isSpeedChange = animationSpeed < _targetSpeed - 0.1f || animationSpeed > _targetSpeed + 0.1f;

        if(isSpeedChange)
        {
            _speed = Mathf.Lerp(animationSpeed, _targetSpeed, 100f * Time.fixedDeltaTime);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = _targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_inputSystem.MoveVector.x, 0f, _inputSystem.MoveVector.y).normalized;

        if(inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, 0.12f);

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        Vector3 moveDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;

        Vector3 moveVleocity = moveDirection.normalized * _speed;

        moveVleocity.y = _rigidbody.linearVelocity.y;

        _rigidbody.linearVelocity = moveVleocity;

        _animator.SetFloat(_animationMovement, _speed);
    }

    private void SettingAction(bool isEnable)
    {
        _inputSystem.SetSprintAction(Sprint, isEnable);
        _inputSystem.SetAttackAction(Attack, isEnable);
        _inputSystem.SetReloadAction(Reload, isEnable);
        _inputSystem.SetZoomAction(Zoom, isEnable);
        if (isLocalPlayer)
        {
            
        }
    }

    private void Sprint(bool isSprint)
    {
        float speed = isSprint ? 5f : 3f;

        _currentMoveSpeed = speed;
    }

    private void Attack(bool isAttack)
    {
        if (isAttack)
        {
            _animator.SetTrigger(_animationFire);
        }
    }

    private void Zoom(bool isZoom)
    {
        _isZoomMode = isZoom;

        _playerCamera.Zoom(isZoom);
    }

    private void Reload(bool isReload)
    {
        if (isReload)
        {
            _animator.SetTrigger(_animationReload);
        }
    }

    private void Movement()
    {

    }
}
