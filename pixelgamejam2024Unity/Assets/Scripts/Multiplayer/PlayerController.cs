using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerCharacterSO _playerSO;
    
    private float _maxSpeed;
    private float _carryCapacity;
    private int _health;
    private int _attack;
    [SerializeField]
    private float _moveForce = 1f;
    private Vector3 _move;
    private Vector3 _forceDir;
    private Rigidbody _rb;
    private bool _facingLeft; //Change according to initial sprite
    
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private PlayerControls _input;
    private Vector2 _movementAxis;
    private Vector2 _mouseDelta;

    [SerializeField]
    private Camera _cam;
    private Transform _camTransform;

    private bool _alive;
    private bool _canControl;

    public bool AliveAndReady => _alive && _canControl;
    

    public void SetAsCharacter(PlayerCharacterSO characterData)
    {
        _playerSO = characterData;
        Cursor.lockState = CursorLockMode.Locked;
        _maxSpeed = _playerSO.MoveSpeed;
        _carryCapacity = _playerSO.CarryCapacity;
        _health = _playerSO.Health;
        _attack = _playerSO.Attack;
        _rb = GetComponent<Rigidbody>();
        _camTransform = _cam.transform;
        _input = new PlayerControls();
        
        _alive = true;
        _canControl = true;
        OnEnable();
    }

    private void OnEnable()
    {
        if (!AliveAndReady) return;
        _input.Gameplay.Enable();
        _input.Gameplay.Movement.performed += OnMovementPressed;
        _input.Gameplay.Movement.canceled += OnMovementReleased;
        _input.Gameplay.MouseLook.performed += OnLook;
        _input.Gameplay.Pickup.performed += OnPickupPressed;
        _input.Gameplay.Attack.performed += OnAttackPressed;
    }

    private void OnDisable()
    {
        _input.Gameplay.Movement.performed -= OnMovementPressed;
        _input.Gameplay.Movement.canceled -= OnMovementReleased;
        _input.Gameplay.MouseLook.performed -= OnLook;
        _input.Gameplay.Pickup.performed -= OnPickupPressed;
        _input.Gameplay.Attack.performed -= OnAttackPressed;
    }

    #region InputSystem

    private void OnMovementPressed(InputAction.CallbackContext obj)
    {
        _movementAxis = obj.ReadValue<Vector2>();
        _move = new Vector3(_movementAxis.x,0, _movementAxis.y);
    }
	
    private void OnMovementReleased(InputAction.CallbackContext obj)
    {
        _movementAxis = obj.ReadValue<Vector2>();
        _move = Vector3.zero;
    }
    
    private void OnLook(InputAction.CallbackContext obj)
    {
        _mouseDelta = obj.ReadValue<Vector2>();
    }
    
    private void OnPickupPressed(InputAction.CallbackContext obj)
    {
        
    }

    private void OnAttackPressed(InputAction.CallbackContext obj)
    {
        
    }
    
    #endregion


    private void FixedUpdate()
    {
        _forceDir += _movementAxis.x * GetCameraRight(_cam) * _moveForce;
        _forceDir += _movementAxis.y * GetCameraForward(_cam) * _moveForce;
        _rb.AddForce(_forceDir, ForceMode.Impulse);
        transform.forward = _forceDir;
        _forceDir = Vector3.zero;

        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0f;
        if (horizontalVelocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            _rb.velocity = horizontalVelocity.normalized * _maxSpeed;
    }

    private Vector3 GetCameraRight(Camera cam)
    {
        Vector3 right = _cam.transform.right;
        right.y = 0f;
        return right.normalized;
    }
    
    private Vector3 GetCameraForward(Camera cam)
    {
        Vector3 forward = _cam.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private void LookAt()
    {
        Vector3 direction = _rb.velocity;
        direction.y = 0f;

        if (_movementAxis.sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        } 
    }

    private void Move(Vector3 move)
    {
        //_rb.velocity = move * _maxSpeed * Time.fixedDeltaTime;
        
        if (move.x > 0 && _facingLeft)
        {
            FlipSprite();
        }
        else if (move.x < 0 && !_facingLeft)
        {
            FlipSprite();
        }
        if (move == Vector3.zero) return;
        _forceDir = _move.normalized;
        transform.forward = _move;
    }
    
    private void FlipSprite()
    {
        _facingLeft = !_facingLeft;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
