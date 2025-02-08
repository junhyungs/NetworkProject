using UnityEngine;
using UnityEngine.UI;

public class GameSettingPanel : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(OnClickExit);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(OnClickExit);
    }

    public void OnClickExit()
    {
        GameUIManager.Instance.OnClickExitGame();
    }
}
