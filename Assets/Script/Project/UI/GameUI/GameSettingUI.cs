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
            Cursor.lockState = CursorLockMode.Locked;

            _settingPanel.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            _settingPanel.SetActive(true);
        }
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
