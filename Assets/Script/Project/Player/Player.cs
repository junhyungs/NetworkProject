using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData _playerData;

    protected PlayerInputSystem _inputSystem;
    protected PlayerCamera _playerCamera;
    protected PlayerGun _gun;
    protected Rigidbody _rigidbody;
    protected Animator _animator;
    protected Camera _camera;

    public PlayerGun Gun
    {
        get => _gun;
        set => _gun = value;
    }

    protected readonly int _animationMovement = Animator.StringToHash("SpeedValue");
    protected readonly int _animationFire = Animator.StringToHash("Fire");
    protected readonly int _animationReload = Animator.StringToHash("Reloading");

    #region SyncProperty
    [SyncVar]
    private float _damage;
    public float SyncDamage
    {
        get => _damage;
        set
        {
            if (isServer)
            {
                _damage = value;
            }
        }
    }

    [SyncVar]
    private float _currentMoveSpeed;
    public float SyncMovespeed
    {
        get => _currentMoveSpeed;
        set
        {
            if (isServer)
            {
                _currentMoveSpeed = value;
            }
        }
    }

    [SyncVar(hook = nameof(HooK_Health))]
    private float _health;
    private void HooK_Health(float _, float value)
    {
        if (isOwned)
        {
            GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.Health, value);
        }
    }
    public float SyncHealth
    {
        get => _health;
        set
        {
            if (isServer)
            {
                _health = value;
            }
        }
    }

    [SyncVar(hook = nameof(Hook_MaxBullet))]
    private int _maxBullet;
    private void Hook_MaxBullet(int _, int value)
    {
        if (isOwned)
        {
            GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.MaxBullet, value);
        }
    }
    public int SyncMaxBullet
    {
        get => _maxBullet;
        set
        {
            if (isServer)
            {
                _maxBullet = value;
            }
        }
    }

    #endregion
    private int _currentBullet = 20;

    private float _targetSpeed;
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _currentZoomRotation;
    private float _zoomRotationVelocity;

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

    private void OnDisable()
    {
        OnDisablePlayer();  
    }

    private void OnDisablePlayer()
    {
        SettingAction(false);
    }

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ReadData();
        SettingAction(true);
    }

    private void ReadData()
    {
        if (_playerData == null || !isOwned)
        {
            return;
        }

        float health = _playerData.Health;
        float damage = _playerData.Damage;
        float moveSpeed = _playerData.MoveSpeed;
        int maxBullet = _playerData.MaxBullet;

        CommandInitializePlayerData(health, damage, moveSpeed, maxBullet);

        _currentBullet = maxBullet;
        GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.CurrentBullet, _currentBullet);
    }

    [Command]
    private void CommandInitializePlayerData(float health, float damage, float moveSpeed, int maxBullet)
    {
        SyncHealth = health;
        SyncMaxBullet = maxBullet;
        SyncDamage = damage;
        SyncMovespeed = moveSpeed;
    }

    private void SettingAction(bool isEnable)
    {
        if (isOwned)
        {
            _inputSystem.SetSprintAction(Sprint, isEnable);
            _inputSystem.SetAttackAction(Attack, isEnable);
            _inputSystem.SetReloadAction(Reload, isEnable);
            _inputSystem.SetZoomAction(Zoom, isEnable);
        }
    }

    private void Update()
    {
        if (isOwned &&_isZoomMode) //줌 모드 이면서 isOwned.
        {
            float mouseDelta = _inputSystem.MouseDelta / 8f;

            _currentZoomRotation += mouseDelta;

            float smoothRotation = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _currentZoomRotation, ref _zoomRotationVelocity, 0.1f);

            transform.rotation = Quaternion.Euler(0f,smoothRotation, 0f);
        }
    }

    private void OnAnimatorMove()
    {
        if (isOwned)
        {
            Movement();
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
            if (!_isZoomMode)
            {
                AttackRotation();
            }
            
            if (_currentBullet <= 0)
            {
                return;
            }

            _currentBullet--;

            GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.CurrentBullet, _currentBullet);

            _gun.Fire();

            _animator.SetTrigger(_animationFire);
        }
    }

    private void AttackRotation()
    {
        float targetAngle = Camera.main.transform.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    private void Zoom(bool isZoom)
    {
        if (isZoom)
        {
            float targetAngle = Camera.main.transform.rotation.eulerAngles.y; //현재 카메라 회전 값.

            _currentZoomRotation = targetAngle; //누적된 회전값을 현재 사용해야하는 회전값으로 초기화.

            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f); //회전.
        }

        _isZoomMode = isZoom;

        _playerCamera.Zoom(isZoom);
    }

    private void Reload(bool isReload)
    {
        if (isReload)
        {
            _currentBullet = _maxBullet;

            GameUIManager.Instance.TriggerPlayerUIEvent(UIEvent.CurrentBullet, _currentBullet);

            CommandReloadAnimation();
        }
    }

    [Command]
    private void CommandReloadAnimation()
    {
        ClientRPCReloadAnimation();
    }

    [ClientRpc]
    private void ClientRPCReloadAnimation()
    {
        _animator.SetTrigger(_animationReload);
    }

    private void Movement()
    {
        _targetSpeed = _currentMoveSpeed;

        if (_inputSystem.MoveVector == Vector2.zero)
        {
            _targetSpeed = 0f;
        }

        float animationSpeed = _animator.deltaPosition.magnitude / Time.fixedDeltaTime;

        bool isSpeedChange = animationSpeed < _targetSpeed - 0.1f || animationSpeed > _targetSpeed + 0.1f;

        if (isSpeedChange)
        {
            _speed = Mathf.Lerp(animationSpeed, _targetSpeed, 100f * Time.fixedDeltaTime);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = _targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_inputSystem.MoveVector.x, 0f, _inputSystem.MoveVector.y).normalized;

        if (inputDirection != Vector3.zero && !_isZoomMode)
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
}
