using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerCharacterSO _playerSO;
    private float _moveSpeed;
    private float _maxSpeed;
    private float _carryCapacity;
    private float _currentCarriedWeight;
    private int _health;
    private int _attack;
    private bool _attackCooldown;
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

    [SerializeField]
    private CapsuleCollider _pickupZone;
    [SerializeField]
    private GameObject _attackZone;

    
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
        Grab();
    }

    private void OnAttackPressed(InputAction.CallbackContext obj)
    {
        if(!_attackCooldown) Attack();
    }
    
    #endregion


    private void FixedUpdate()
    {
        _forceDir += _movementAxis.x * GetCameraRight(_cam) * _moveForce;
        _forceDir += _movementAxis.y * GetCameraForward(_cam) * _moveForce;
        //transform.forward = _forceDir;
        _rb.AddForce(_forceDir, ForceMode.Impulse);
        _forceDir = Vector3.zero;

        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0f;
        if (horizontalVelocity.sqrMagnitude > _moveSpeed * _moveSpeed)
            _rb.velocity = horizontalVelocity.normalized * _moveSpeed;
        
        LookAt();
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
        else
        {
            _rb.angularVelocity = Vector3.zero;
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

    private void Grab()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, _pickupZone.radius);
        foreach (Collider2D collider2D in collider2Ds)
        {
            if (collider2D.GetComponent<Pickup>() != null)
            {
                if (_currentCarriedWeight >= _carryCapacity)
                {
                    //Print fail message
                    return;
                }
                collider2D.GetComponent<Pickup>()?.Interact();
                //Pickup object; add weight
                SetMaxSpeed();
                //FixedJoint2D fj = transform.gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
            }

            if (collider2D.GetComponent<Conveyour>() != null && _currentCarriedWeight > 0)
            {
                //Unload
                SetMaxSpeed();
            }
        }
    }

    private void SetMaxSpeed()
    {
        if (_currentCarriedWeight <= _carryCapacity * 0.25f)
        {
            _moveSpeed = _maxSpeed; 
        }else if (_currentCarriedWeight <= _carryCapacity * 0.5f)
        {
            _moveSpeed = _maxSpeed * 0.75f;
        }else if (_currentCarriedWeight <= _carryCapacity * 0.75f)
        {
            _moveSpeed = _maxSpeed * 0.5f;
        }
        else
        {
            _moveSpeed = _maxSpeed * 0.25f;
        }
    }

    private async void Attack()
    {
        _attackZone.SetActive(true);
        _attackCooldown = true;
        await Task.Delay(250);
        _attackZone.SetActive(false);
        _attackCooldown = false;
    }
}
