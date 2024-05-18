using Multiplayer;
using Playroom;
using UnityEngine;

public class KoboldAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private PlayerNetworkControllerV2 _playerNetworkController;
    
    private Camera _cam;
    private Vector3 _posLastFrame = Vector3.zero;

    // Update is called once per frame
    void FixedUpdate()
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
        var playerMovement = posThisFrame - _posLastFrame;

        // Movement is relative to main player camera
        var camTransform = _cam.transform;
        var camForward = camTransform.forward;
        var camRight = camTransform.right;
        
        var xMove = Vector3.Dot(playerMovement, camRight);
        var zMove = Vector3.Dot(playerMovement, camForward);
        
        _spriteRenderer.flipX = xMove < 0f;
        _animator.SetFloat("Horizontal", xMove); // left or right of us
        _animator.SetFloat("Vertical", zMove); // towards or away from us
        _animator.SetFloat("Speed", playerMovement.magnitude); // did they move
        
        bool isCarrying = false; // todo local player
        
        if (PlayroomKit.IsRunningInBrowser())
            isCarrying = _playerNetworkController.RepresentsPlayer.GetState<bool>(
                GameConstants.PlayerStateData.IsCarrying.ToString());
        
        _animator.SetBool("Carrying", isCarrying); // carry state flag
        
        _posLastFrame = posThisFrame;
    }
}
