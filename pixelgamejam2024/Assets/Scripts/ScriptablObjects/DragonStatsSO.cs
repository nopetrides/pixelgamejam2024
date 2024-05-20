using System;
using Multiplayer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Dragon/DragonStatus")]
public class DragonStatsSO : ScriptableObject
{
	[SerializeField]
	private GameConstants.DragonStats _stat;
	public GameConstants.DragonStats Stat => _stat;
	
	[SerializeField]
	private int _max;
	public int Max => _max;
	[NonSerialized] public int Current = 0;
	[NonSerialized] public int ChangeThisFrame = 0;
}
