using System;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("CountUI")]
    [SerializeField] private PlayerCountUI _playerCountUI;
    public PlayerCountUI PlayerCountUI => _playerCountUI;

    [Header("PlayerUIPanelObject")]
    [SerializeField] private GameObject _playerPanel;
    public GameObject PlayerPanel => _playerPanel;
}
