using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRDebug;

public class InventorySystem : MonoBehaviour
{
	GameController _game;
	MenuController _menuController;
	public int _kills = 0;
	public int maxLevel = 5;
	public List<int> _dragonIndexes = new List<int>(3) { };
	public List<int> _xp = new List<int>(8) { };
	private List<int> _currentLevelXp = new List<int>(8) { 0, 0, 0, 0, 0};
	private List<int> _maxLevelXp = new List<int>(8) { };
	[SerializeField] public List<int> _levelsXp = new List<int>(6) { };
	public List<int> _strength = new List<int>(8) { };
	public List<int> _hp = new List<int>(8) { };
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_menuController = FindAnyObjectByType<MenuController>();
	}
	public int CalculateLevel(int id)
	{
		int xp = _xp[id];
		int level = 0;
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
		if (CalculateCurrentLevelXp(id) >= _levelsXp[_levelsXp.Count-1])
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
		int _currentLevel = CalculateLevel(id);
		_xp[id] += amount;
		if (CalculateLevel(id) > _currentLevel)
		{
			Debug.Log("Leveled up");
			_strength[id] *= 2;
			if (CalculateLevel(id) != 3 && CalculateLevel(id) != 5)
				StartCoroutine(_game._cdController.LevelUp());
			if (CalculateLevel(id) > 1)
			{
				_hp[id] += 50;
			}
			if (CalculateLevel(id) == 3)
			{
				_dragonIndexes.Add(_dragonIndexes.Max()+1);
				_game.SwitchGrowth();
			}
			if (CalculateLevel(id) == 5)
			{
				_game.SwitchGrowth();
			}
		}
		else
		{
			_currentLevelXp[id] = CalculateCurrentLevelXp(id);
		}
		_menuController.UpdateDragonsDisplay();
		_menuController.UpdateEnemyDragonsDisplay();
	}
}
