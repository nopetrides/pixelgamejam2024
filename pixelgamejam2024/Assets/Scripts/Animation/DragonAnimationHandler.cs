using System;
using UnityEngine;
using UnityEngine.UI;

public class DragonAnimationHandler : MonoBehaviour
{
    [SerializeField] private DragonNetworkController _dragonNetworkController;
    [SerializeField] private Animator _dragonAnimator;
    [SerializeField] private Image _uiImage;

    private int _previousDragonAge;
    private DragonNetworkController.FiniteDragonState _previousDragonState;

    private void Start()
    {
        if (_uiImage != null)
        {
            _uiImage.SetNativeSize();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        var state = _dragonNetworkController.DragonState;
        if (state != _previousDragonState)
        {
            _dragonAnimator.SetInteger("State", (int)state);
            _previousDragonState = state;
            if (_uiImage != null)
            {
                _uiImage.SetNativeSize();
            }
        }
    }
}
