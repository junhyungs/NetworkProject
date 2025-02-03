using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private GameObject _IdObject;
    [SerializeField] private GameObject _passwordObject;
    [SerializeField] private GameObject _buttonObject;

    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _passwordField;

    private void Start()
    {
        InitializeLoginPanel();
    }

    #region Panel
    private void InitializeLoginPanel()
    {
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
}
