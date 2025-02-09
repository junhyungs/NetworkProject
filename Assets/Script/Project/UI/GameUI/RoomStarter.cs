using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mirror;
using UnityEngine.InputSystem;

public class RoomStarter : NetworkBehaviour
{
    [Header("StartButton")]
    [SerializeField] private Button _startHostButton;

    [Header("SubmitAction")]
    [SerializeField] private InputActionReference _submitAction;

    [Header("Count")]
    [SerializeField] private int _count;

    [Header("CountDownUI")]
    [SerializeField] private GameObject _uiObject;

    [Header("CountDownText")]
    [SerializeField] private Text _countDownText;

    private void Start()
    {
        if (isServer)
        {
            _startHostButton.gameObject.SetActive(true);

            _submitAction.action.Enable();

            _submitAction.action.performed += SubMitAction;
        }
    }

    private void SubMitAction(InputAction.CallbackContext context)
    {
        if (isServer)
        {
            _startHostButton.onClick.Invoke();

            _startHostButton.gameObject.SetActive(false);
        }
    }

    [Server]
    private IEnumerator CountDown(int count)
    {
        ClientRPC_ActiveCountDownUI(true);

        while (count > 0)
        {
            ClientRPC_CountDown(count);

            yield return new WaitForSeconds(1f);

            count--;
        }

        ClientRPC_ActiveCountDownUI(false);

        var roomManager = Project_RoomManager.Instance;

        foreach(var networkPlayer in roomManager.roomSlots)
        {
            var roomPlayer = networkPlayer as Project_RoomPlayer;

            roomPlayer.ReadyToBegin();
        }

        roomManager.ServerChangeScene(roomManager.GameplayScene);
    }

    [ClientRpc]
    private void ClientRPC_ActiveCountDownUI(bool isActive)
    {
        _uiObject.SetActive(isActive);
    }


    public void StartGame()
    {
        StartCoroutine(CountDown(_count));
    }

    [ClientRpc]
    private void ClientRPC_CountDown(int count)
    {
        _countDownText.text = $"게임 시작까지 <color=#FF0000>{count}</color>초 전...";
    }
}
