using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
	GameController _game;
	MenuController _menuController;
	public int money = 0;
	public int maxLevel = 5;
	public List<int> _dragonIndexes = new List<int>(3) { 0, 1, 2 };
	public List<int> _xp = new List<int>(8) { 0, 0, 0, 0, 0, 0, 0, 0 };
	private List<int> _currentLevelXp = new List<int>(8) { 0, 0, 0, 0, 0, 0, 0, 0 };
	private List<int> _maxLevelXp = new List<int>(8) { 0, 0, 0, 0, 0, 0, 0, 0 };
	[SerializeField] public List<int> _levelsXp = new List<int>(6) { 10, 20, 50, 100 };
	public List<int> _strength = new List<int>(8) { 5, 5, 5, 5, 5, 5, 5, 5 };
	public List<int> _hp = new List<int>(8) { 1000, 100, 100, 100, 100, 100, 100, 100 };
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_menuController = FindAnyObjectByType<MenuController>();
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
		if (_currentLevelXp[id] >= _levelsXp[_levelsXp.Count-1])
			return _levelsXp.Count;
		if (level == 1 && xp == 0)
			return 0;
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
		if (xp > _levelsXp[_levelsXp.Count-1])
			return _levelsXp[_levelsXp.Count-1];
		return xp;
	}
	public int CalculateMaxLevelXp(int id)
	{
		return _levelsXp[CalculateLevel(id)];
	}
	public void GainXp(int id, int amount)
	{
		_xp[id] += amount;
		if (CalculateCurrentLevelXp(id) >= CalculateMaxLevelXp(id))
		{
			_strength[id] *= 3;
			_game._cdController.LevelUp();
			if (CalculateLevel(id) > 1)
			{
				_hp[id] += 50;
				_strength[id] += 5;
			}
			if (CalculateLevel(id) == 3)
			{
				_dragonIndexes.Add(_dragonIndexes.Max()+1);
			}
		}
		else
		{
			_currentLevelXp[id] = CalculateCurrentLevelXp(id);
			// instantiate effect for gain xp;
		}
		_menuController.UpdateDragonsDisplay();
	}
}
