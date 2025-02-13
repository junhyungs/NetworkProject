using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("CountDownUI")]
    [SerializeField] private GameObject _countDownUI;
    [SerializeField] private Text _countDownText;

    [Header("CurrentLevelUI")]
    [SerializeField] private GameObject _currentLevelUI;
    [SerializeField] private Text _currentLevelText;
    [SerializeField] private float _duractionTime;
    public Text CountDownText => _countDownText;
    public Text CurrentLevelText => _currentLevelText;
   
    public void SetText(int currentLevel)
    {
        _currentLevelText.text = $"Level <color=#FF0000>{currentLevel}</color>";
    }

    public void SetAlpha(float alpha)
    {
        Color color = _currentLevelText.color;

        color.a = alpha;    

        _currentLevelText.color = color;
    }

    public void ActiveCountDownUI(bool isActive)
    {
        _countDownUI.SetActive(isActive);
    }

    public void ActiveLevelUI(bool isActive)
    {
        _currentLevelUI.SetActive(isActive);
    }

    public void CountDown(int count)
    {
        _countDownText.text = $"다음 공격까지 <color=#FF0000>{count}</color>초";
    }

}
