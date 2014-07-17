using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class WaveMaster : MonoBehaviour {

	public GUISkin window;
	public GUISkin bigText;
	public List<GameObject> enemyUnits = new List<GameObject>();
	public float waveDelay = 3;

	private Level _currentLevel = new Level();
	private bool _loadFail = false;
	private bool _enemySpawned = false;
	private bool _waveClear = true;
	private float _nextWaveTime = 0;
	private int _curWave = 0;
	private List<GameObject> _spawnedEnemies = new List<GameObject>();
	private int _enemyCount = 0;
	private bool _levelClear = false;
	private bool _gameOver = false;
	private ControlMap _controls;
	// Use this for initialization
	void Start () 
	{
		if(Application.isEditor)
		{
			//PlayerPrefs.SetString("loadPath", "C:/Users/Khamraj/Documents/New Unity Project/Assets/Levels/Custom/New Level.lvl");
		}
		_controls = GameObject.Find("_GameRegistry").GetComponent<ControlMap>();
		if(!Load(PlayerPrefs.GetString("loadPath")))
		{
			_loadFail = true;
		}
		PlayerPrefs.SetString("loadPath", "");
		_nextWaveTime = Time.time + waveDelay;
		if(_loadFail)
			Destroy(GameObject.FindGameObjectWithTag("Player"));
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.SetString("loadPath","");
	}
	// Update is called once per frame
	void Update () 
	{
		if(!_loadFail)
		{
			if(!_levelClear)
			{
				CountEnemies();
				if(!_enemySpawned && _nextWaveTime <= Time.time && !_waveClear)
				{
					_waveClear = false;
					Wave thisWave = _currentLevel.Waves[_curWave];
					int c = 0;
					_spawnedEnemies.Clear();
					foreach(int i in thisWave.UnitID)
					{
						GameObject spawn = Instantiate(enemyUnits[i], new Vector3(thisWave.UnitPos[c].x, thisWave.UnitPos[c].y, 0), Quaternion.identity) as GameObject;
						_spawnedEnemies.Add(spawn);
						c++;
					}
					_enemySpawned = true;
					CountEnemies();
				}
				if(_enemyCount == 0 && !_waveClear && _enemySpawned)
				{
					_nextWaveTime = Time.time + waveDelay;
					_waveClear = true;
					_curWave++;
				}
				if(_waveClear && _nextWaveTime <= Time.time)
				{
					_enemySpawned = false;
					_waveClear = false;
				}
			}
			if(_curWave > _currentLevel.Waves.Count-1)
				_levelClear = true;
			if(_levelClear || _gameOver)
			{
				if(Input.GetKeyDown(_controls.GetKey("Advance")))
					Application.LoadLevel("MainMenu");
			}
		}
	}

	void CountEnemies()
	{
		_enemyCount = GameObject.FindGameObjectsWithTag("enemy").Length;
	}

	public void PlayerDeath()
	{
		_gameOver = true;
	}

	void OnGUI()
	{
		float hw = Screen.width/2;
		float hh = Screen.height/2;
		hh -= 64;
		if(_loadFail)
		{
			GUI.skin = window;
			GUI.Window(0, new Rect(hw-256, hh-150, 512,300), ErrorWin, "Error");
		}else
		{
			GUI.skin = bigText;
			if(_waveClear && !_levelClear && !_gameOver)
			{
				GUI.Label(new Rect(0, hh, Screen.width, 64), _currentLevel.Waves[_curWave].WaveName);
				GUI.Label(new Rect(0, hh+64, Screen.width, 64), "Starting in: " + Round(_nextWaveTime-Time.time, 10));
			}
			if(_levelClear)
				GUI.Label(new Rect(0, hh, Screen.width, 64), "Level Complete!");
			if(_gameOver)
				GUI.Label(new Rect(0, hh, Screen.width, 64), "Game Over!");
			if(_gameOver || _levelClear)
				GUI.Label(new Rect(0, hh+64, Screen.width, 64), "Press " + _controls.GetKey("Advance").ToString() + " to continue!");
		}
	}

	float Round(float n, int d)
	{
		return (Mathf.Round(n*d)/d);
	}
	void ErrorWin(int ID)
	{
		float left = 50;
		float top = 50;
		float pad = 5;
		float lh = 64;
		float width = (512)-100;
		GUI.Label(new Rect(left, top+((pad+lh)*.7f), width, lh), "Unable to load the level.");
		if(GUI.Button(new Rect(left, top+((pad+lh)*2), width, lh), "Main Menu"))
		{
			Application.LoadLevel("MainMenu");
		}
	}

	bool Load(string path)
	{
		Debug.Log(path);
		if(path == null || path == "")
			return false;
		if(File.Exists(path))
		{
			PlayerPrefs.SetString("loadPath", "");
			return _currentLevel.SetLevelData(File.ReadAllLines(path));
		}else
			return false;
	}

	public List<GameObject> GetSpawnedUnits()
	{
		return _spawnedEnemies;
	}
}
