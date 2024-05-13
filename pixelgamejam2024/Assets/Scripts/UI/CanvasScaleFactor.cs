using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
[ExecuteAlways]
public class CanvasScaleFactor : MonoBehaviour
{
    [SerializeField] private float _referenceWidth = 640f;
    [SerializeField] private float _referenceHeight = 360f;
    
    private CanvasScaler _canvasScaler;
    private float _previousWidth;
    private float _previousHeight;


    private void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        if (_canvasScaler == null)
        {
            Debug.LogError("Canvas Scaler component not found on this object.");
            enabled = false;
            return;
        }

        UpdateScale();
    }

    private void OnEnable()
    {
        UpdateScale();
    }

    private void Update()
    {
        if (Math.Abs(_previousWidth - Screen.width) > 1 && Math.Abs(_previousHeight - Screen.height) > 1)
            UpdateScale();
    }

    private void UpdateScale()
    {
        _previousWidth = Screen.width;
        _previousHeight = Screen.height;

        float scaleFactor = Mathf.Min(_previousWidth / _referenceWidth, _previousWidth / _referenceHeight);
        _canvasScaler.scaleFactor = Mathf.Max(1f, Mathf.FloorToInt(scaleFactor));
        Debug.Log($"{_previousHeight} / {_referenceWidth} x {_previousHeight} / {_referenceHeight} scale {scaleFactor}");
    }
}