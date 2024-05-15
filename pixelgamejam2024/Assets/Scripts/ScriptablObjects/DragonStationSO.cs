using Multiplayer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hub/DragonStation")]
public class DragonStationSO : ScriptableObject
{
    [SerializeField] private GameConstants.DragonStats _affectsDragonStats;

    [SerializeField] private int _affectValue;
    

    public GameConstants.DragonStats AffectsDragonStats
    {
        get => _affectsDragonStats;
        private set => _affectsDragonStats = value;
    }
    
    public int AffectValue
    {
        get => _affectValue;
        private set => _affectValue = value;
    }
}
