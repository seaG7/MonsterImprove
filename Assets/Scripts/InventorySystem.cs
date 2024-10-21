using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
	GameController _game;
	public int money = 0;
	public List<int> _dragonIndexes = new List<int>(1) { 0 };
	public List<int> _xp = new List<int>(6) { 0, 0, 0, 0, 0, 0};
	public List<int> _strength = new List<int>(6) { 0, 0, 0, 0, 0, 0};
	public List<int> _hp = new List<int>(6) { 0, 0, 0, 0, 0, 0};
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
	}
	void Update()
	{
		
	}
	public void LoadStats(int id)
	{
		DragonBehaviour _dragonController = _game._currentDragon.GetComponent<DragonBehaviour>();
		_dragonController._xp = _xp[id];
		_dragonController._strength = _strength[id];
		_dragonController._hp = _hp[id];
		Debug.Log("Stats loaded");
	}
	public void SaveStats(int id)
	{
		DragonBehaviour _dragonController = _game._currentDragon.GetComponent<DragonBehaviour>();
		_xp[id] = _dragonController._xp;
		_strength[id] = _dragonController._strength;
		_hp[id] = _dragonController._hp;
		Debug.Log("Stats saved");
	}
}
