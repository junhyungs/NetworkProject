using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountUI : NetworkBehaviour
{
    [Header("CountText")]
    [SerializeField] private Text _playerCountText;

    [SyncVar(hook = nameof(Hook_PlayerCount))]
    public int _currentPlayerCount;
    private void Hook_PlayerCount(int _, int value)
    {
        _playerCountText.text = $"현재 인원 {value}명";
    }
}
