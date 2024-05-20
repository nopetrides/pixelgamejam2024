using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private DragonNetworkController _dragonController;
    [SerializeField] private TextAnimator_TMP _textAnimator;
    [SerializeField] private TMP_Text _text;
    private Color _overheatColor = new Color((float)0.15, (float)0.53, (float)0.80);
    private Color _crankyColor = new Color((float)0.56, (float)0.81, (float)0.2);
    private Color _tiredColor = new Color((float)0.8, (float)0.66, (float)0.92);
    private Color _idleColor = Color.white;
    private Color _currentColor;
    private int _speed = 8;

    private void Update()
    {
        _text.color = Color.Lerp(_idleColor, _currentColor, Mathf.Sin(Time.time * _speed));

        if (_dragonController.DragonState == DragonNetworkController.FiniteDragonState.Overheated)
        {
            _textAnimator.enabled = true;
            _currentColor = _overheatColor;
        }
        else if (_dragonController.DragonState == DragonNetworkController.FiniteDragonState.Cranky)
        {
            _textAnimator.enabled = true;
            _currentColor = _crankyColor;
        }
        else if (_dragonController.DragonState == DragonNetworkController.FiniteDragonState.Sleeping)
        {
            _textAnimator.enabled = true;
            _currentColor = _tiredColor;
        }
        else
        {
            _currentColor = _idleColor;
            _textAnimator.enabled = false;
        }
    }
}
