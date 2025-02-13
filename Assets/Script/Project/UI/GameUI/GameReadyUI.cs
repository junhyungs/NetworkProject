using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameReadyUI : MonoBehaviour
{
    [Header("GameReadyUIObject")]
    [SerializeField] private GameObject _gameReadyUIObject;

    [Header("ValueText")]
    [SerializeField] private Text _valueText;

    public void OnReadyUI(bool isOn)
    {
        _gameReadyUIObject.SetActive(isOn);

        if (isOn)
        {
            StartCoroutine(ReadyUICoroutine());
        }
        else
        {
            StopCoroutine(ReadyUICoroutine());  
        }
    }

    private IEnumerator ReadyUICoroutine()
    {
        string[] pointArray = new string[] { " ", ".", "..", "...", "....", "...", "..", ".", " " };

        int arrayIndex = 0;
        
        while (true)
        {
            _valueText.text = pointArray[arrayIndex];

            arrayIndex = (arrayIndex + 1) % pointArray.Length;

            yield return new WaitForSeconds(0.5f);
        }
    }
}
