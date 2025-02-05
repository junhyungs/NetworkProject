using UnityEngine;
using UnityEngine.UI;

public class SignupPanel : MonoBehaviour
{
    [Header("SuccessUI")]
    [SerializeField] private GameObject _duplicateSuccessUI;
    [SerializeField] private GameObject _createIDUI;

    [Header("ErrorUI")]
    [SerializeField] private GameObject _duplicateErrorUI;
    [SerializeField] private GameObject _createIDErrorUI;

    [Header("Create_ID_Button")]
    [SerializeField] private Button _createIdButton;

    [Header("InputField")]
    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _passwordField;

    private void OnDisable()
    {
        FieldStringEmpty();
    }

    private void FieldStringEmpty()
    {
        _idField.text = string.Empty;

        _passwordField.text = string.Empty;
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }

    public void OnClickCreateID()
    {
        if (IsNotNullOrEmpty())
        {
            var uiManager = LoginUIManager.Instance;

            uiManager.SignUp_CreateID(_idField.text, _passwordField.text,
                CreateIDCallBack);
        }
    }

    private void CreateIDCallBack(ReceiveSignUpMessage message)
    {
        if(message._code == AuthenticatorCode.Success)
        {
            _createIDUI.SetActive(true);

            FieldStringEmpty();
        }
        else if(message._code == AuthenticatorCode.Fail)
        {
            _createIDErrorUI.SetActive(true);
        }
    }

    private bool IsNotNullOrEmpty()
    {
        if(string.IsNullOrEmpty(_idField.text) ||
            string.IsNullOrEmpty(_passwordField.text))
        {
            return false;
        }

        return true;
    }

    public void OnClickDuplicateCheck()
    {
        if(IsNotNullOrEmpty())
        {
            var uiManager = LoginUIManager.Instance;

            uiManager.SignUp_DuplicateCheck(_idField.text, _passwordField.text,
                DuplicateCheckCallBack);
        }
    }

    private void DuplicateCheckCallBack(ReceiveDuplicateMessage message)
    {
        if(message._code == AuthenticatorCode.Success)
        {
            _duplicateSuccessUI.SetActive(true);

            _createIdButton.interactable = true;
        }
        else if(message._code == AuthenticatorCode.Fail)
        {
            _duplicateErrorUI.SetActive(true);
        }
    }
}
