using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountUI : MonoBehaviour
{
    [Header("CountText")]
    [SerializeField] private Text _playerCountText;

    public void PlayerCount(int count)
    {
        _playerCountText.text = $"���� �ο� {count}��";
    }
}
