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

        if (!PlayroomKit.IsRunningInBrowser())
            Mock();
        else
            CheckNetwork();
        
    }

    private void Mock()
    {
        var posThisFrame = _playerNetworkController.transform.position;
        var playerMovement = posThisFrame - _posLastFrame;

        var camTransform = _cam.transform;
        var camForward = camTransform.forward;
        var camRight = camTransform.right;
        
        var xMove = Vector3.Dot(playerMovement, camRight);
        var zMove = Vector3.Dot(playerMovement, camForward);
        
        // Movement is relative to main player camera
        _animator.SetFloat("Horizontal", xMove); // left or right of us
        _spriteRenderer.flipX = xMove < 0f;
        _animator.SetFloat("Vertical", zMove); // towards or away from us
        _animator.SetFloat("Speed", playerMovement.magnitude); // did they move
        // todo get carry state locally
        _animator.SetBool("Carrying", false); // carry state flag
        
        _posLastFrame = posThisFrame;
    }
    
    private void CheckNetwork()
    {
        var posThisFrame = _playerNetworkController.RepresentsPlayer.GetState<Vector3>(GameConstants.PlayerStateData.Position.ToString());
        var playerMovement = posThisFrame - _posLastFrame;

        var camTransform = _cam.transform;
        var camForward = camTransform.forward;
        var camRight = camTransform.right;

        var xMove = Vector3.Dot(playerMovement, camRight);
        var zMove = Vector3.Dot(playerMovement, camForward);
        
        // Movement is relative to main player camera
        _animator.SetFloat("Horizontal", xMove); // left or right of us
        _spriteRenderer.flipX = xMove < 0f;
        _animator.SetFloat("Vertical", zMove); // towards or away from us
        _animator.SetFloat("Speed", playerMovement.magnitude); // did they move
        var isCarrying = _playerNetworkController.RepresentsPlayer.GetState<bool>(GameConstants.PlayerStateData.IsCarrying.ToString());
        _animator.SetBool("Carrying", isCarrying); // carry state flag

        _posLastFrame = posThisFrame;
    }
}
