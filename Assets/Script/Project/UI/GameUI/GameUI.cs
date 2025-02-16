using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("CountUI")]
    [SerializeField] private PlayerCountUI _playerCountUI;
    public PlayerCountUI PlayerCountUI => _playerCountUI;

    [Header("PlayerUIPanelObject")]
    [SerializeField] private GameObject _playerPanel;
    public GameObject PlayerPanel => _playerPanel;

    [Header("PlayerDeathUI")]
    [SerializeField] private GameObject _deathUI;
    [SerializeField] private Button _regameButton;
    public void DeathUI()
    {
        bool isActive = _deathUI.activeSelf ? false : true;

        _deathUI.SetActive(isActive);
    }

    public void OnRegameButton(bool isServer)
    {
        if (isServer)
        {
            _regameButton.gameObject.SetActive(true);
        }
    }
}
