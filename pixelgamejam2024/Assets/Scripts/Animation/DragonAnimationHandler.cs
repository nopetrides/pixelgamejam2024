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

        _dragonNetworkController.OnDragonDataRefresh += CheckDragonAnimations;
    }

    // Update is called once per frame
    private void CheckDragonAnimations(DragonData dragonData)
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

        var age = dragonData.Age;
        if (age > _previousDragonAge)
        {
            _dragonAnimator.SetInteger("Age", (int)state);
            _previousDragonAge = age;
        }
    }
}
