using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [Header("LodingUI")]
    [SerializeField] private LoginLoadingUI _loginLoadingUI;

    [Header("ErrorUI")]
    [SerializeField] private GameObject _loginErrorUI;

    [Header("UI_Object")]
    [SerializeField] private GameObject _IdObject;
    [SerializeField] private GameObject _passwordObject;
    [SerializeField] private GameObject _buttonObject;

    [Header("InputField")]
    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _passwordField;

    private void Start()
    {
        InitializeLoginPanel();
    }

    private void OnDisable()
    {
        InitializeLoginPanel();
    }

    #region Panel
    private void InitializeLoginPanel()
    {
        _idField.text = string.Empty;
        _passwordField.text = string.Empty;

        _passwordObject.SetActive(false);
        _buttonObject.SetActive(false);
    }

    public void OnValueChangedIdField()
    {
        bool idFieldNullOrEmpty = !string.IsNullOrEmpty(_idField.text);

        bool passwordNullOrEmpty = string.IsNullOrEmpty(_passwordField.text);

        if (!idFieldNullOrEmpty && passwordNullOrEmpty)
        {
            _passwordObject.SetActive(false);

            _buttonObject.SetActive(false);

            return;
        }
        
        _passwordObject.SetActive(true);
    }

    public void OnValueChangedPasswordField()
    {
        bool passwordFieldNullOrEmpty = !string.IsNullOrEmpty(_passwordField.text);

        bool idFieldNullOrEmpty = string.IsNullOrEmpty(_idField.text);

        if(!passwordFieldNullOrEmpty && idFieldNullOrEmpty)
        {
            _passwordObject.SetActive(false);

            _buttonObject.SetActive(false);

            return;
        }

        _buttonObject.SetActive(passwordFieldNullOrEmpty);
    }
    #endregion

    public void OnClickLogin()
    {
        bool isIdFieldNullOrEmpty = string.IsNullOrEmpty(_idField.text);
        bool isPasswordFieldNullOrEmpty = string.IsNullOrEmpty(_passwordField.text);

        if (!isIdFieldNullOrEmpty && !isPasswordFieldNullOrEmpty)
        {
            StartLodingUI();

            var uiManager = LoginUIManager.Instance;

            uiManager.Login(_idField.text, _passwordField.text, LoginCallBack);
        }
    }

    private void StartLodingUI()
    {
        _loginLoadingUI.gameObject.SetActive(true);

        _loginLoadingUI.StartColorChangeCoroutine(0.2f);
    }

    private void LoginCallBack(ReceiveAuthenticatorMessage message)
    {
        StartCoroutine(DeleyCoroutine(message));
    }

    private IEnumerator DeleyCoroutine(ReceiveAuthenticatorMessage message)
    {
        yield return new WaitForSeconds(2f);

        LoadingUIControll();

        if (message._code == AuthenticatorCode.Success)
        {
            LoginUIManager.Instance.ActiveOnlineUI(true);

            gameObject.SetActive(false);
        }
        else if (message._code == AuthenticatorCode.Fail)
        {
            _loginErrorUI.SetActive(true);
        }
    }

    private void LoadingUIControll()
    {
        _loginLoadingUI.StopAllCoroutines();

        _loginLoadingUI.ResetColor();

        _loginLoadingUI.gameObject.SetActive(false);
    }
}
