using System;
using AOT;
using CMF;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerNetworkControllerV2 : MonoBehaviour
{
    [SerializeField] private AdvancedWalkerController _controller;
    [SerializeField] private CharacterInput _inputHandler;
    [SerializeField] private GameObject _cameraRoot;
    [SerializeField] private PlayerTreasurePickup _treasurePickup;
    public PlayerTreasurePickup LocalPickupLogic => _treasurePickup;
    // Give the game a momment to catch up after loading
    // playroom kit bug?
    [SerializeField] private float _sceneLoadDelay = 1f;
    //[SerializeField] private PlayerHealth _playerHealth;
    //[SerializeField] private HighlightEffect _highlight;
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private SmoothPosition _positionSmoother;
    [SerializeField] private Camera _primaryCamera;
    public Camera MainPlayerCamera => _primaryCamera;
    [SerializeField] private Camera _minimapCamera;

    // todo, player controller that handles the other game mechanics
    
    private PlayroomKit.Player _playroomPlayer;
    public PlayroomKit.Player RepresentsPlayer => _playroomPlayer;
    private GameStateManager _manager;
    private float _warmTimer;
    private bool _warmedUp;
    
    public void Setup(PlayroomKit.Player player, GameStateManager manager)
    {
        Debug.Log("[PlayerNetworkControllerV2] Setting up networked player controller");
        _playroomPlayer = player;
        _manager = manager;
        var characterTypeState = player.GetState<int>(GameConstants.PlayerStateData.CharacterType.ToString());
        var characterData = PlayerCharactersData.Characters[(GameConstants.CharacterTypes)characterTypeState];
        if (characterData == null)
        {
            Debug.LogError($"Failed to determine character for type {characterTypeState}");
            return;
        }

        bool isLocalPlayer = !PlayroomKit.IsRunningInBrowser() || _playroomPlayer == PlayroomKit.Me();
        SetAsPlayer(isLocalPlayer);
        SetAsCharacter(isLocalPlayer, characterData);
    }

    private void SetAsPlayer(bool isLocalPlayer)
    {
        _positionSmoother.enabled = !isLocalPlayer;
        _cameraRoot.SetActive(isLocalPlayer);
        if (!PlayroomKit.IsRunningInBrowser()) return;
        _playroomPlayer.OnQuit(RemovePlayer);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private void RemovePlayer(string playerID)
    {
        Destroy(gameObject);
    }

    private void SetAsCharacter(bool isLocalPlayer, PlayerCharacterSO characterData)
    {
        Debug.Log($"{characterData.MoveSpeed}");
        _controller.movementSpeed = characterData.MoveSpeed;
        Debug.Log($"{characterData.MoveSpeed}");
        _controller.baseSpeed = characterData.MoveSpeed;
        _treasurePickup.SetLimitAndThreshold(characterData.CarryCapacity);
        /*
        if (!isLocalPlayer)
        {
            //_highlight.highlighted = false;
            //_playerSpriteRenderer.color = characterData.CharacterColor;
        }
        else
        {
            //_highlight.highlighted = true;
            //_highlight.overlayColor = characterData.CharacterColor;
        }
        */
        //_playerHealth.MaxHealth = characterData.Health;
        // todo, other data driven fields - modify a different controller that handles player stats like hp and carrying.
    }

    public void Update()
    {
        if (!_warmedUp)
        {
            _warmTimer += Time.deltaTime;
            if (_warmTimer > _sceneLoadDelay)
                _warmedUp = true;
            return;
        }

        if (!PlayroomKit.IsRunningInBrowser()) return;
        
        if (_playroomPlayer == PlayroomKit.Me())
        {
            // we are this player
            _playroomPlayer.SetState(GameConstants.PlayerStateData.Position.ToString(), transform.position);

            float scaleFactor = Mathf.Min(Screen.width / 640, Screen.height / 360);
            
            // Set the camera's viewport rect to position it in the bottom right corner
            float normalizedWidth = (100f * Mathf.Max(1f, Mathf.FloorToInt(scaleFactor))) / Screen.width;
            float normalizedHeight = (100f * Mathf.Max(1f, Mathf.FloorToInt(scaleFactor))) / Screen.height;
            _minimapCamera.rect = new Rect(1 - normalizedWidth, 0, normalizedWidth, normalizedHeight);
            return;
        }

        if (_playroomPlayer == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = _playroomPlayer.GetState<Vector3>(GameConstants.PlayerStateData.Position.ToString());
    }
}
