using UnityEngine;
using UnityEngine.UI;

public class SignupPanel : MonoBehaviour
{
    [SerializeField] private Button _createIdButton;
    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _passwordField;


    public void OnClickExitButton()
    {
        _idField.text = string.Empty;
        _passwordField.text = string.Empty;

        gameObject.SetActive(false);
    }

    public void OnClickDuplicateCheck()
    {
        bool isIdFieldEmpty = string.IsNullOrEmpty(_idField.text);
        bool isPasswordFieldEmpty = string.IsNullOrEmpty(_passwordField.text);

        if(!isIdFieldEmpty && !isPasswordFieldEmpty)
        {
            
        }
    }

    private void DuplicateCheckCallBack(ReceiveDuplicateMessage message)
    {

    }
}
