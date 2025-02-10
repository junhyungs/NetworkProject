using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mirror;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class RoomStarter : NetworkBehaviour
{
    [Header("Text")]
    [SerializeField] private GameObject _readyUI;
    [SerializeField] private Text _readyText;

    [Header("CurrentReadyText")]
    [SerializeField] private GameObject _currentReadyUI;
    [SerializeField] private Text _currentReadyText;

    [Header("SubmitAction")]
    [SerializeField] private InputActionReference _submitAction;

    [Header("Count")]
    [SerializeField] private int _count;

    [Header("CountDownUI")]
    [SerializeField] private GameObject _uiObject;

    [Header("CountDownText")]
    [SerializeField] private Text _countDownText;

    private Project_RoomManager _roomManager;

    private int _readyPlayerCount;
    private bool _allClientReady;
    private bool _isReady;
    private bool _prvReady;


    private void Start()
    {
        EnableAction();
        StartServer();
        StartClient();
    }

    private void Update()
    {
        if (isServer)
        {
            bool isStart = _roomManager.ReadyPlayerCount == NetworkServer.connections.Count;

            _allClientReady = isStart ? true : false;

            if (_allClientReady)
            {
                ServerText("ENTER로 게임 시작");
            }
            else
            {
                ServerText("아직 모든 Player가 준비되지 않았습니다.");
            }
        }
    }

    private void EnableAction()
    {
        _submitAction.action.Enable();
        _submitAction.action.performed += SubMitAction;
    }

    private void DisableAction()
    {
        _submitAction.action.performed -= SubMitAction;
        _submitAction.action.Disable();
    }

    private void SubMitAction(InputAction.CallbackContext context)
    {
        if (isServer && _allClientReady)
        {
            StartGame();
        }
        else if(isClient && !isServer)
        {
            _isReady = _prvReady ? false : true;

            _prvReady = _isReady;

            ClientReady(_isReady);
        }
    }

    #region Server
    private void StartServer()
    {
        if (isServer)
        {
            _currentReadyUI.gameObject.SetActive(true);

            StartCoroutine(HostPlayerReady());
        }
    }

    private void CurrentReadyText(int count)
    {
        _currentReadyText.text = $"현재 준비된 인원 수 {count}명";
    }

    private IEnumerator HostPlayerReady()
    {
        _roomManager = Project_RoomManager.Instance;

        _roomManager._readyPlayerCountCallBack += CurrentReadyText;

        yield return new WaitUntil(() => { return _roomManager.roomSlots.Count > 0; });

        _roomManager.SetReadyPlayerCount(true);
    }

    private void ServerText(string text)
    {
        _readyText.text = text;
    }

    [Server]
    private void StartGame()
    {
        StartCoroutine(CountDown(_count));

        DisableAction();
    }

    [Server]
    private IEnumerator CountDown(int count)
    {
        _readyUI.SetActive(false);

        _currentReadyUI.SetActive(false);

        ClientRPC_DisableUI();

        ClientRPC_ActiveCountDownUI(true);

        while (count > 0)
        {
            ClientRPC_CountDown(count);

            yield return new WaitForSeconds(1f);

            count--;
        }

        ClientRPC_ActiveCountDownUI(false);

        var hostPlayer = Project_RoomPlayer.MyPlayer;

        hostPlayer.ReadyToBegin(true);
    }

    #endregion

    #region Client
    private void StartClient()
    {
        if (isClient && !isServer)
        {
            _readyText.text = "ENTER로 준비";
        }
    }

    [Client]
    private void ClientReady(bool isReady)
    {
        var myPlayer = Project_RoomPlayer.MyPlayer;

        myPlayer.ReadyToBegin(isReady);

        myPlayer.CommandRoomPlayerReady(isReady);

        _readyText.text = isReady ? "Ready" : "ENTER로 준비";
    }

    [ClientRpc]
    private void ClientRPC_DisableUI()
    {
        _readyUI.SetActive(false);
        
        DisableAction();
    }

    [ClientRpc]
    private void ClientRPC_ActiveCountDownUI(bool isActive)
    {
        _uiObject.SetActive(isActive);
    }

    [ClientRpc]
    private void ClientRPC_CountDown(int count)
    {
        _countDownText.text = $"게임 시작까지 <color=#FF0000>{count}</color>초 전...";
    }
    #endregion
}
