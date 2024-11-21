using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
	GameController _game;
	public int money = 0;
	public int maxLevel = 5;
	public List<int> _dragonIndexes = new List<int>(1) { 0 };
	public List<int> _xp = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
	private List<int> _currentLevelXp = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
	private List<int> _maxLevelXp = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
	[SerializeField] public List<int> _levelsXp = new List<int>(6) { 10, 20, 50, 100 };
	public List<int> _strength = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
	public List<int> _hp = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
	}
	public int CalculateLevel(int id)
	{
		int xp = _xp[id];
		int level = 1;
		foreach (int amount in _levelsXp)
		{
			if (xp >= amount)
			{
				xp -= amount;
				level++;
			}
			else
				break;
		}
		if (_currentLevelXp[id] >= _levelsXp[_levelsXp.Count])
			return _levelsXp.Count;
		return level;
	}
	public int CalculateCurrentLevelXp(int id)
	{
		int xp = _xp[id];
		foreach (int amount in _levelsXp)
		{
			if (xp >= amount)
				xp -= amount;
			else
				break;
		}
		if (xp > _levelsXp[_levelsXp.Count])
			return _levelsXp[_levelsXp.Count];
		return xp;
	}
	public int CalculateMaxLevelXp(int id)
	{
		return _levelsXp[CalculateLevel(id)-1];
	}
	public void GainXp(int id, int amount)
	{
		_xp[id] += amount;
		if (_currentLevelXp[id] >= _maxLevelXp[id])
		{
			_game._cdController.LevelUp();
		}
		else
		{
			_currentLevelXp[id] = CalculateCurrentLevelXp(id);
			// instantiate effect for gain xp;
		}
	}
}
