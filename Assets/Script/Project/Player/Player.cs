using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData _playerData;

    private PlayerInputSystem _inputSystem;
    private PlayerCamera _playerCamera;
    private PlayerGun _gun;
    private Rigidbody _rigidbody;
    private Camera _camera;
    protected PlayerInput _playerInput;
    protected Animator _animator;

    private readonly int _animationMovement = Animator.StringToHash("SpeedValue");
    private readonly int _animationFire = Animator.StringToHash("Fire");
    private readonly int _animationReload = Animator.StringToHash("Reloading");
    private readonly int _animationWalk = Animator.StringToHash("Walk");
    private readonly int _animationRun = Animator.StringToHash("Run");
    protected readonly int _animationDie = Animator.StringToHash("Die");

    private int _currentBullet = 20;
    private float _targetSpeed;
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _currentZoomRotation;
    private float _zoomRotationVelocity;
    private bool _isZoomMode;

    [SyncVar(hook = nameof(Hook_Damage))]
    private float _damage;
    [SyncVar(hook = nameof(Hook_Speed))]
    private float _currentMoveSpeed;
    [SyncVar(hook = nameof(HooK_Health))]
    private float _health;
    [SyncVar(hook = nameof(Hook_MaxBullet))]
    private int _maxBullet;

    public PlayerGun Gun
    {
        get => _gun;
        set => _gun = value;
    }

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

    public float SyncHealth
    {
        get => _health;
        set
        {
            if (isServer)
            {
                _health = value;

                NetworkUIManager.Instance.UpdatePartyUI(netId, _health);
            }
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
    
    private void Hook_Damage(float _, float value)
    {
        if (isOwned)
        {
            UIManager.Instance.TriggerUIEvent(UIEvent.Damage, value);
        }
    }
      
    private void Hook_Speed(float _, float value)
    {
        if (isOwned)
        {
            UIManager.Instance.TriggerUIEvent(UIEvent.Speed, value);
        }
    }
   
    private void HooK_Health(float _, float value)
    {
        if (isOwned)
        {
            UIManager.Instance.TriggerUIEvent(UIEvent.Health, value);
        }
    }

    private void Hook_MaxBullet(int _, int value)
    {
        if (isOwned)
        {
            UIManager.Instance.TriggerUIEvent(UIEvent.MaxBullet, value);
        }
    }    

    public override void OnStopServer()
    {

    }

    private void Awake()
    {
        InitializeOnAwakePlayer();
    }

    private void InitializeOnAwakePlayer()
    {
        _playerInput = GetComponent<PlayerInput>();
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
        UIManager.Instance.TriggerUIEvent(UIEvent.CurrentBullet, _currentBullet);
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
        _animator.SetBool(_animationRun, isSprint);

        if (isSprint)
        {
            CommandSetSprintSpeed(2f);
        }
        else
        {
            CommandSetSprintSpeed(-2f);
        }
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

            UIManager.Instance.TriggerUIEvent(UIEvent.CurrentBullet, _currentBullet);

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

            UIManager.Instance.TriggerUIEvent(UIEvent.CurrentBullet, _currentBullet);

            CommandReloadAnimation();
        }
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

        AnimationMovement();
    }

    private void AnimationMovement()
    {
        bool moveAnimationControl = _inputSystem.MoveVector != Vector2.zero;
        bool isRun = _animator.GetBool(_animationRun);

        if (!moveAnimationControl && isRun)
        {
            _animator.SetBool(_animationRun, moveAnimationControl);
        }

        _animator.SetBool(_animationWalk, moveAnimationControl);
    }

    [Command]
    private void CommandInitializePlayerData(float health, float damage, float moveSpeed, int maxBullet)
    {
        SyncHealth = health;
        SyncMaxBullet = maxBullet;
        SyncDamage = damage;
        SyncMovespeed = moveSpeed;
    }

    [Command]
    private void CommandReloadAnimation()
    {
        ClientRPCReloadAnimation();
    }

    [Command]
    private void CommandSetSprintSpeed(float speedValue)
    {
        _currentMoveSpeed += speedValue;
    }


    [ClientRpc]
    private void ClientRPCReloadAnimation()
    {
        _animator.SetTrigger(_animationReload);
    }
}
