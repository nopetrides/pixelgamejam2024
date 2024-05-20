using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Multiplayer;
using Playroom;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Slider[] _statVisuals;
    [SerializeField] private DragonNetworkController _dragonController;
    [SerializeField] private TMP_Text _dragonStateText;
    [SerializeField] private TMP_Text _weightText;
    [SerializeField] private GameObject[] _playerContainers;
    [FormerlySerializedAs("_portaits")] [SerializeField] private Image[] _portraits;

    [SerializeField] private RectTransform _minimap;
    [SerializeField] private RectTransform _minimapFacingVisual;
    [SerializeField] private RectTransform _minimapDragonIndicator;

    [Header("HP")] [SerializeField] private Image _hpFill;
    [Header("Growth")] [SerializeField] private TMP_Text _growthText;

    private PlayerNetworkControllerV2 _localPlayer;

    private readonly Dictionary<string, Slider> _statusSliders = new();

    // todo make a custom monoBehaviour instead of image
    private readonly Dictionary<string, Image> _playerPortraits = new();

    public void SetPlayer(PlayerNetworkControllerV2 localPlayerController)
    {
        _localPlayer = localPlayerController;
        var players = PlayroomKit.GetPlayersOrNull();
        if (players == null)
        {
            SetupPortrait(_localPlayer);
            return;
        }
        
        SetupPlayerPortraits(players.Values.ToArray());
    }

    private void SetupPortrait(PlayerNetworkControllerV2 mockLocalPlayer)
    {
        for (var i = 0; i < _portraits.Length; i++)
        {
            var image = _portraits[i];
            if (i == 0)
            {
                image.gameObject.SetActive(true);
                _playerPortraits.Add("Mock", image);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    private void SetupPlayerPortraits(PlayroomKit.Player[] players)
    {
        for (var i = 0; i < _portraits.Length; i++)
        {
            var image = _portraits[i];
            if (i < players.Length)
            {
                image.gameObject.SetActive(true);
                _playerPortraits.Add(players[i].id, image);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        Debug.Log("GameUI Start");
        _dragonController.OnDragonDataRefresh += DragonStatusRefresh;

        if (_statVisuals.Length < 4)
        {
            Debug.LogError("Not enough slider visuals assigned!");
        }
        _statusSliders.Add(GameConstants.DragonStats.Heat.ToString(), _statVisuals[0]);
        _statusSliders.Add(GameConstants.DragonStats.Temper.ToString(), _statVisuals[1]);
        _statusSliders.Add(GameConstants.DragonStats.Energy.ToString(), _statVisuals[2]);
        _statusSliders.Add(GameConstants.DragonStats.Chewing.ToString(), _statVisuals[3]);
        
        _hpFill.fillAmount = 1f;

        _growthText.text = "";
    }

    private void DragonStatusRefresh(DragonData dragonData)
    {
        foreach (var kvp in dragonData.CurrentAgeData.CurrentStats)
        {
            _statusSliders[kvp.Key].value = (float)kvp.Value.Current / kvp.Value.Max;
        }

        var state = _dragonController.DragonState;
        
        _dragonStateText.text = state != DragonNetworkController.FiniteDragonState.Idle ? _dragonController.DragonState.ToString() : "Normal";

        _hpFill.fillAmount = (float)dragonData.MaxHealth / dragonData.Health;
        
        _growthText.text = $"{dragonData.Growth} / {dragonData.CurrentAgeData.GrowthRequirement}";
    }

    private void Update()
    {
        if (_localPlayer == null) return;
        MinimapIndicators();
        _weightText.text = $"{_localPlayer.LocalPickupLogic.GetWeight()} / {_localPlayer.LocalPickupLogic.GetCarryLimit()} lbs";
    }

    private void MinimapIndicators()
    {
        DragonDirectionIndicator();

        FacingDirectionIndicator();
    }

    /// <summary>
    /// Direction to Dragon indicator
    /// </summary>
    private void DragonDirectionIndicator()
    {
        Vector3 playerPosition = _localPlayer.transform.position;
        /*if (!PlayroomKit.IsRunningInBrowser())
            playerPosition = _localPlayer.transform.position;
        else
        {
            playerPosition = PlayroomKit.Me().GetState<Vector3>(GameConstants.PlayerStateData.Position.ToString());
        }*/

        var dPos = _dragonController.transform.position;

        Vector2 dirToDragon = new Vector2(playerPosition.x - dPos.x, playerPosition.z - dPos.z);
        Vector2 dragonDirNormal = dirToDragon.normalized;

        var rect = _minimap.rect;
        Vector2 minimapCenter = rect.center;
        var minimapSize = rect.size;
        float mapWidth = minimapSize.x / 2;
        float mapHeight = minimapSize.y / 2;

        float scale = Mathf.Min(mapWidth / Mathf.Abs(dragonDirNormal.x), mapHeight / Mathf.Abs(dragonDirNormal.y));
        Vector2 edgePosition = minimapCenter + dragonDirNormal * scale;

        edgePosition.x = Mathf.Clamp(edgePosition.x, -minimapSize.x, minimapSize.x / 2);
        edgePosition.y = Mathf.Clamp(edgePosition.y, -minimapSize.y / 2, minimapSize.y);

        _minimapDragonIndicator.transform.localPosition = edgePosition;
        _minimapDragonIndicator.transform.up = dirToDragon;
    }
    
    /// <summary>
    ///     Facing Indicator
    /// </summary>
    private void FacingDirectionIndicator()
    {
        if (_localPlayer == null)
        {
            Debug.LogWarning("FacingDirectionIndicator player null");
            return;
        }

        var camForward = _localPlayer.MainPlayerCamera.transform.forward;
        Vector2 cameraForward2D = new Vector2(camForward.x, camForward.z).normalized;
        Vector2 northDirection = Vector2.down;
        float angle = Vector2.SignedAngle(northDirection, cameraForward2D);
        _minimapFacingVisual.localRotation = Quaternion.Euler(0, 0, angle);
    }

}
