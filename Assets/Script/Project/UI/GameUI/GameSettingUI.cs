using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettingUI : MonoBehaviour
{
    [Header("ESC_Action")]
    [SerializeField] private InputActionReference _escAction;

    [Header("SettingPanel")]
    [SerializeField] private GameObject _settingPanel;

    private void OnEnable()
    {
        _escAction.action.Enable();

        _escAction.action.performed += ActiveSettingPanel;
    }

    private void OnDisable()
    {
        _escAction.action.performed -= ActiveSettingPanel;  

        _escAction.action.Disable();
    }

    private void ActiveSettingPanel(InputAction.CallbackContext callbackContext)
    {
        if (_settingPanel.activeSelf)
        {
            _settingPanel.SetActive(false);
        }
        else
        {
            _settingPanel.SetActive(true);
        }
    }
}
