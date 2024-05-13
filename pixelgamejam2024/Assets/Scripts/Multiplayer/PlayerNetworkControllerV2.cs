using CMF;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerNetworkControllerV2 : MonoBehaviour
{
    [SerializeField] private AdvancedWalkerController _controller;
    [SerializeField] private CharacterInput _inputHandler;
    [SerializeField] private GameObject _cameraRoot;
    // Give the game a momment to catch up after loading
    // playroom kit bug?
    [SerializeField] private float _sceneLoadDelay = 1f;
    [SerializeField] private PlayerHealth _playerHealth;

    
    // todo, player controller that handles the other game mechanics
    
    private PlayroomKit.Player _playroomPlayer;
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

        SetAsPlayer();
        SetAsCharacter(characterData);
    }

    private void SetAsPlayer()
    {
        bool isLocalPlayer = !PlayroomKit.IsRunningInBrowser() || _playroomPlayer == PlayroomKit.Me();
        _cameraRoot.SetActive(isLocalPlayer);
        if (!isLocalPlayer)
        {
            DestroyImmediate(_inputHandler);
        }
    }

    private void SetAsCharacter(PlayerCharacterSO characterData)
    {
        _controller.movementSpeed = characterData.MoveSpeed;
        _playerHealth.MaxHealth = characterData.Health;
        // todo, other data driven fields - modify a different controller that handles player stats like hp and carrying.
    }

    public void LateUpdate()
    {
        if (!_warmedUp)
        {
            _warmTimer += Time.deltaTime;
            if (_warmTimer > _sceneLoadDelay)
                _warmedUp = true;
            return;
        }
        if (!PlayroomKit.IsRunningInBrowser() || _playroomPlayer == PlayroomKit.Me())
        {
            // we are this player
            _playroomPlayer.SetState(GameConstants.PlayerStateData.Position.ToString(), transform.position);
            return;
        }

        transform.position = _playroomPlayer.GetState<Vector3>(GameConstants.PlayerStateData.Position.ToString());
    }
}
