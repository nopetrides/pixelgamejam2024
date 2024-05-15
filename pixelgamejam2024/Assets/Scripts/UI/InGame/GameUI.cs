using System.Collections.Generic;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Slider[] _statVisuals;
    [SerializeField] private DragonNetworkController _dragonController; // temp;
    [SerializeField] private TMP_Text _dragonStateText;

    private Dictionary<string, Slider> _statusSliders = new ();
    private void Start()
    {
        // temp
        _dragonController.OnDragonDataRefresh += DragonStatusRefresh;

        if (_statVisuals.Length < 4)
        {
            Debug.LogError("Not enough slider visuals assigned!");
        }
        _statusSliders.Add(GameConstants.DragonStats.Heat.ToString(), _statVisuals[0]);
        _statusSliders.Add(GameConstants.DragonStats.Temper.ToString(), _statVisuals[1]);
        _statusSliders.Add(GameConstants.DragonStats.Energy.ToString(), _statVisuals[2]);
        _statusSliders.Add(GameConstants.DragonStats.Chewing.ToString(), _statVisuals[3]);
    }

    private void DragonStatusRefresh(Dictionary<string, DragonNetworkController.DragonStatus> dragonData)
    {
        foreach (var kvp in dragonData)
        {
            _statusSliders[kvp.Key].value = (float)kvp.Value.Current / kvp.Value.Max;
        }
        
        _dragonStateText.text = _dragonController.DragonStateDebug;
    }
}
