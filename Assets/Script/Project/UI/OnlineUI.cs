using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    [Header("NickNameInputField")]
    [SerializeField] private InputField _nickNameField;

    [Header("Button")]
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;

    private void OnEnable()
    {
        _nickNameField.onValueChanged.AddListener(OnValueChangedNickNameField);

        _startHostButton.onClick.AddListener(OnClickStartHost);
        _startClientButton.onClick.AddListener(OnClickStartClient);
    }

    private void OnDisable()
    {
        _nickNameField.onValueChanged.RemoveListener(OnValueChangedNickNameField);

        _startHostButton.onClick.RemoveListener(OnClickStartHost);
        _startClientButton.onClick.RemoveListener(OnClickStartClient);
    }

    private void OnClickStartHost()
    {
        PlayerInformation.NickName = _nickNameField.text;

        var roomManager = Project_RoomManager.Instance;

        roomManager.StartHost();
    }

    private void OnClickStartClient()
    {
        PlayerInformation.NickName = _nickNameField.text;

        var roomManager = Project_RoomManager.Instance;

        roomManager.StartClient();
    }

    private void OnValueChangedNickNameField(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            SetButton(true);
        }
        else
        {
            SetButton(false);
        }
    }

    private void SetButton(bool isInteractable)
    {
        _startHostButton.interactable = isInteractable;
        _startClientButton.interactable = isInteractable;

        _startHostButton.image.color = isInteractable ? Color.red : Color.white;
        _startClientButton.image.color = isInteractable ? Color.red : Color.white;
    }
}
