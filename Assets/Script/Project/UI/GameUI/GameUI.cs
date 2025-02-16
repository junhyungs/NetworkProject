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
    public GameObject DeathUI => _deathUI;
    
    /// <summary>
    /// Server
    /// </summary>
    public void OnRegameButton()
    {
        Cursor.lockState = CursorLockMode.None;

        _regameButton.gameObject.SetActive(true);        
    }

    public void OnClickRegame()
    {
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance.ReGame();
    }
}
