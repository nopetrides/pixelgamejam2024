using DG.Tweening;
using Febucci.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SplashScreenUI : MonoBehaviour
{
    [SerializeField] private string[] _characterBlurbs;
    [SerializeField] private TypewriterByCharacter[] _typewriters;
    [SerializeField] private CanvasGroup[] _canvasGroups;
    [SerializeField] private CanvasGroup _onCompleteCanvas;

    private int _section = -1;

    private void Start()
    {
        NextCanvas();
    }


    private void NextCanvas()
    {
        _section++;
        if (_section >= _canvasGroups.Length)
        {
            ShowEnd();
            return;
        }
        
        _canvasGroups[_section].DOFade(1f, 1f).OnComplete(NextTypewriter);
    }

    private void NextTypewriter()
    {
        _typewriters[_section].onTextShowed.AddListener(NextCanvas);
        _typewriters[_section].ShowText(_characterBlurbs[_section]);
        _typewriters[_section].StartShowingText(true);
    }

    private void ShowEnd()
    {
        _onCompleteCanvas.DOFade(1f, 1f);
    }
    
    public void ButtonSkip()
    {
        LoadingManager.Instance.LoadScene("MainMenuScene");
    }
}
