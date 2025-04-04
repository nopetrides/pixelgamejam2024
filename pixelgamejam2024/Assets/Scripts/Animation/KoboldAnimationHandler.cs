using System;
using Multiplayer;
using Playroom;
using UnityEngine;

public class KoboldAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _spriteColored;
    [SerializeField] private SpriteRenderer _carriedTreasure;

    [SerializeField] private PlayerNetworkControllerV2 _playerNetworkController;
    [SerializeField] private PlayerTreasurePickup _playerTreasurePickup; 
    
    private Camera _cam;
    private Vector3 _posLastFrame = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (_cam == null)
            try
            {
                _cam = Camera.main;
            }
            catch
            {
                Debug.LogWarning("Camera not yet ready");
            }

        AnimationStateUpdate();
    }

    private void AnimationStateUpdate()
    {
        var posThisFrame = _playerNetworkController.transform.position;
        var playerMovement = (posThisFrame - _posLastFrame).normalized;

        // Movement is relative to main player camera
        var camTransform = _cam.transform;
        var camForward = camTransform.forward;
        var camRight = camTransform.right;
        
        var xMove = Math.Round(Vector3.Dot(playerMovement, camRight), 2);
        var zMove =  Math.Round(Vector3.Dot(playerMovement, camForward), 2);

        _spriteRenderer.flipX = xMove < 0f;
        _spriteColored.flipX = xMove < 0f;
        
        
        _animator.SetFloat("Horizontal", (float)xMove); // left or right of us
        _animator.SetFloat("Vertical", (float)zMove); // towards or away from us
        _animator.SetFloat("Speed", playerMovement.magnitude); // did they move
        
        bool isCarrying = false; // todo local player
        
        if (PlayroomKit.IsRunningInBrowser())
        {
            isCarrying = _playerNetworkController.RepresentsPlayer.GetState<bool>(
                GameConstants.PlayerStateData.IsCarrying.ToString());
		}
		
        if (!PlayroomKit.IsRunningInBrowser())
            isCarrying = _playerTreasurePickup.IsCarrying();
        
        
        _animator.SetBool("Carrying", isCarrying); // carry state flag
        _carriedTreasure.gameObject.SetActive(isCarrying);
        if (isCarrying)
        {
            _carriedTreasure.flipX = xMove < 0f;
            var t = _carriedTreasure.transform.localPosition;
            t.z = zMove > 0f ? -0.15f : 0.15f;
            _carriedTreasure.transform.localPosition = t;
        }

        _posLastFrame = posThisFrame;
    }
}
