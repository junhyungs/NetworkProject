using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    [Header("Fill_Image")]
    [SerializeField] private Image _fillImagge;
    [Header("NameText")]
    [SerializeField] private Text _nameText;

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    public void FillHp(float hp)
    {
        var hpfill = hp / 100f;

        _fillImagge.fillAmount = hpfill;
    }
}
