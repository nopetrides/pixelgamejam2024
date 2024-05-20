using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Dragon/DragonAge")]
public class DragonAgeSO : ScriptableObject
{
	[SerializeField]
	private DragonStatsSO[] _stats;
	
	// Needed For Growth
	[SerializeField] 
	private int _growthRequirement;

	public int GrowthRequirement => _growthRequirement;
	
	private Dictionary<string, DragonStatsSO> _currentStats;
	
	public Dictionary<string, DragonStatsSO> CurrentStats => _currentStats;
	
	public void SetupStats()
	{
		if (!Application.isPlaying) return;
		_currentStats = new();
		foreach(var stats in _stats) 
			_currentStats.Add(stats.Stat.ToString(), stats);
		Debug.Assert(_currentStats.Count == Enum.GetValues(typeof(GameConstants.DragonStats)).Length, "Incorrect number of stats assigned to dragon");
	}
}
