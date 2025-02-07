using Unity.Cinemachine;
using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private GameObject _gunCamera;

    [Header("Deleta")]
    [SerializeField] private float _value;

    private PlayerInputSystem _playerInputSystem;
    private Quaternion _rotation;
    private bool _isZoomMode;

    private void Awake()
    {
        OnAwake();
    }

    private void OnAwake()
    {
        _playerInputSystem = GetComponentInParent<PlayerInputSystem>();

        _rotation = transform.rotation;
    }

    private void Start()
    {
        if (isOwned)
        {
            SetCamera();
        }
    }

    private void SetCamera()
    {
        var camera = Instantiate(_playerCamera);

        CinemachineCamera cinemachineCamera = camera.GetComponent<CinemachineCamera>();

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Target.TrackingTarget = transform;
        }
    }

    private void Update()
    {
        if (_isZoomMode)
        {
            return;
        }

        CameraRotation();
    }

    private void CameraRotation()
    {
        Vector3 currentEulerAngles = _rotation.eulerAngles;

        currentEulerAngles.y += _playerInputSystem.MouseDelta / _value;

        _rotation.eulerAngles = currentEulerAngles;

        transform.rotation = _rotation;
    }

    public void Zoom(bool isZoom)
    {
        _isZoomMode = isZoom;

        ZoomActionCamera(isZoom);
    }

    private void ZoomActionCamera(bool isActive)
    {
        _playerCamera.SetActive(!isActive);

        _gunCamera.SetActive(isActive);
    }
}
