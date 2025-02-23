using UnityEngine;
using System.Collections.Generic;

public class PartyUIController : MonoBehaviour
{
    [Header("PartyUI")]
    [SerializeField] private PartyUI[] _partys;

    private int _partysIndex;

    private Dictionary<uint, PartyUI> _partyUIDictionary = new Dictionary<uint, PartyUI>();

    public void MakePartyUI(PartyPlayer partyPlayerData)
    {
        bool isLength = _partysIndex >= _partys.Length;
        bool contains = _partyUIDictionary.ContainsKey(partyPlayerData.NetId);

        if (contains || isLength)
        {
            return;
        }

        var partysUI = _partys[_partysIndex++];

        partysUI.SetName(partyPlayerData.Name);
        partysUI.FillHp(partyPlayerData.Health);

        partysUI.gameObject.SetActive(true);
        _partyUIDictionary.Add(partyPlayerData.NetId, partysUI);
    }

    public void UpdatePartyUI(uint netId, float health)
    {
        if(_partyUIDictionary.TryGetValue(netId, out PartyUI partyUI))
        {
            partyUI.FillHp(health);
        }
    }

    public void DeletePartyUI(uint netId)
    {
        if(_partyUIDictionary.TryGetValue(netId, out PartyUI partyUI))
        {
            partyUI.SetName(string.Empty);
            partyUI.FillHp(0);

            partyUI.gameObject.SetActive(false);
        }
    }
}
