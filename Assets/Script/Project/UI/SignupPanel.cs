using UnityEngine;
using UnityEngine.UI;

public class SignupPanel : MonoBehaviour
{
    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _passwordField;

    public void OnClickExitButton()
    {
        _idField.text = string.Empty;
        _passwordField.text = string.Empty;

        gameObject.SetActive(false);
    }
}
