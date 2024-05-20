using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Dragon/DragonData")]
public class DragonData : ScriptableObject
{
	[SerializeField]
	private DragonAgeSO[] _dragonAges;

	[SerializeField] 
	private int _maxHealth;
	
	public DragonAgeSO[] DragonAges => _dragonAges;

	public int MaxHealth => _maxHealth;
	
	[NonSerialized] 
	public int Age = 0;
	
	[NonSerialized]
	public int Growth = 0;

	[NonSerialized] 
	public int Health = 0;
	
	public DragonAgeSO CurrentAgeData => _dragonAges[Age];
}
