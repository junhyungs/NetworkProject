using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountUI : MonoBehaviour
{
    [Header("CountText")]
    [SerializeField] private Text _playerCountText;

    public void PlayerCount(int count)
    {
        _playerCountText.text = $"현재 인원 {count}명";
    }
}
