using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginLoadingUI : MonoBehaviour
{
    [SerializeField] private Image[] _lodingImage;

    private Coroutine _colorChangeCoroutine;

    private Color _changeColor = new Color(0f, 0f, 0f);
    private Color[] _originalColors;

    private void Awake()
    {
        _originalColors = new Color[_lodingImage.Length];

        for(int i = 0; i < _lodingImage.Length; i++)
        {
            _originalColors[i] = _lodingImage[i].color;
        }
    }

    public void StartColorChangeCoroutine(float waitTime)
    {
        if(_colorChangeCoroutine != null)
        {
            StopCoroutine(_colorChangeCoroutine);

            _colorChangeCoroutine = null;
        }

        _colorChangeCoroutine = StartCoroutine(ColorChange(waitTime));

    }

    public void ResetColor()
    {
        for(int i = 0; i < _lodingImage.Length; i++)
        {
            _lodingImage[i].color = _originalColors[i];
        }
    }

    public IEnumerator ColorChange(float waitTime)
    {    
        int index = 0;

        while(true)
        {
            _lodingImage[index].color = _changeColor;

            int prevIndex = (index - 1 < 0) ? _lodingImage.Length - 1 :
                index - 1;

            _lodingImage[prevIndex].color = _originalColors[prevIndex];

            index = (index + 1) % _lodingImage.Length;

            yield return new WaitForSeconds(waitTime);
        }
    }
}
