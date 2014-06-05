using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	public float animSpeed = 1f;
	public GUISkin skinCore;
	public GUISkin norm;
	public GUISkin window;
	public GUISkin list;
	public float startAngle = 20;
	public float angleInterval;
	public float l2AngleInterval = 5;
	public Texture2D burst;
	public float burstInit = .5f;
	public float burstFinal = 5f;
	public Texture2D selctorRing;
	public List<string> MenuItemNames = new List<string>();

	private float _offset = 0f;
	private float _coreOffset = 0f;
	private List<float> _l2Offset = new List<float>();
	private float _burstScale = 0.5f;
	private bool _menuOpen = false;
	private bool _menuDisplay = false;
	private bool _isBurt = false;
	private int _selectedItem = -1;
	private float _selectorPos = 0;
	private Vector2 _pivot;
	private List<float> _windowOffset = new List<float>();
	private bool _windowOpen = false;
	private bool _windowDisplay = false;
	private int _activeWindow = 0;
	private string _newLevelName = "New Level";
	private List<Level> _customLevels = new List<Level>();
	private List<Level> _mainLevels = new List<Level>();
	private bool firstLaunch = true;
	private string _playerName = "Player";
	private bool _reload = false;
	private List<Vector2> _windowScrollPos = new List<Vector2>();
	private float _selectorSize;
	private DataMap _data;
	private ControlMap _controls;
	//Gampelay Settings
	private bool _mouseMode = false;
	//Audio Settings
	private float _bgmVol = 100;
	private float _sfxVol = 100;
	private float _masterVol = 100;
	//Video Settings
	private Resolution[] _resolutions;
	private bool _vSync = false;
	private int _resIndex = 0;
	private int _resWidth;
	private int _resHeight;
	private int _resRefresh;
	private int _pixelLights;
	private int _AA;
	private bool _fullscreen;
	private bool _aniso;
	private int _texQuality;

	// Use this for initialization
	void Start () 
	{
		LoadSettings();
		for(int i = 0; i < MenuItemNames.Count; i++)
		{
			_l2Offset.Add(0f);
		}
		for(int i = 0; i < 9; i++)
		{
			_windowOffset.Add(0f);
		}
		for(int i = 0; i < 8; i++)
		{
			_windowScrollPos.Add(new Vector2());
		}
		if(PlayerPrefs.GetInt("first?") == 0)
		{
			firstLaunch = true;
		}
		else
		{
			firstLaunch = false;
			_playerName = PlayerPrefs.GetString("name");
		}
		GetLevels();
	}
	void LoadSettings()
	{
		GameObject gameReg = GameObject.Find("_GameRegistry");
		_data = gameReg.GetComponent<DataMap>();
		_controls = gameReg.GetComponent<ControlMap>();
		//Gameplay
		_mouseMode = _data.GetBool("MouseMode");
		//Audio
		_bgmVol = _data.GetFloat("BGM_Vol");
		_sfxVol = _data.GetFloat("SFX_Vol");
		_masterVol = _data.GetFloat("Master_Vol");
		//Video
		_vSync = _data.GetBool("VSync");
		_resHeight = _data.GetInt("Res_Height");
		_resWidth = _data.GetInt("Res_Width");
		_resRefresh = _data.GetInt("Res_Refresh");
		_AA = _data.GetInt("Anti_Aliasing");
		_aniso = _data.GetBool("Aniso_Filtering");
		_texQuality = _data.GetInt("Tex_Quality");
		_fullscreen = _data.GetBool("Fullscreen");
		_resolutions = Screen.resolutions;

		int i = 0;
		foreach(Resolution r in _resolutions)
		{
			Debug.Log(r.width +":"+_resWidth+ " | " + r.height + ":"+ _resHeight + " | " + r.refreshRate +":"+ _resRefresh);
			if(r.width == _resWidth)
			{
				if(r.height == _resHeight)
				{
					if(r.refreshRate == _resRefresh)
					{
						_resIndex = i;
						break;
					}
				}
			}
			i++;
		}
	}
	void OnApplicationQuit()
	{
		PlayerPrefs.SetString("loadPath","");
	}
	void DeleteLevel(int i)
	{
		File.Delete(Application.dataPath+"/Levels/Custom/"+_customLevels[i].LevelName+".lvl");
		_reload = true;
	}
	void GetLevels()
	{
		Debug.Log("Loading Levels");
		_customLevels.Clear();
		_mainLevels.Clear();
		string customDir = Application.dataPath+"/Levels/Custom/";
		if(!Directory.Exists(customDir))
		{
			Directory.CreateDirectory(customDir);
		}
		string[] files = Directory.GetFileSystemEntries(customDir);
		foreach(string f in files)
		{
			Level lvl = new Level();
			if(lvl.SetLevelData(File.ReadAllLines(f)))
				_customLevels.Add(lvl);
		}
		string mainDir = Application.dataPath+"/Levels/Main/";
		if(!Directory.Exists(mainDir))
		{
			Directory.CreateDirectory(mainDir);
		}
		files = Directory.GetFileSystemEntries(mainDir);
		foreach(string f in files)
		{
			Level lvl = new Level();
			lvl.SetLevelData(File.ReadAllLines(f));
			_mainLevels.Add(lvl);
		}
		_reload = false;
	}
	void Update()
	{
		if(_reload)
			GetLevels();
	}
	void LoadGUISkins()
	{
		GUI.skin = skinCore;
		GUI.skin = window;
		GUI.skin = list;
		GUI.skin = norm;
	}

	void OnGUI() 
	{
		LoadGUISkins();
		Rect buttonPos;
		_pivot = new Vector2(0,0);
		if(_menuOpen)
			_offset = Mathf.Lerp(_offset, 1, Time.deltaTime * animSpeed);
		else
			_coreOffset = Mathf.Lerp(_coreOffset, 0, Time.deltaTime * animSpeed);
		if(_menuOpen && !_menuDisplay)
		{
			_offset = 0f;
			_menuDisplay = true;
		}
		if(!_menuOpen && _menuDisplay)
		{
			_offset = Mathf.Lerp(_offset, -1, Time.deltaTime * animSpeed);
			_selectorPos = Mathf.Lerp(_selectorPos, 0, Time.deltaTime * animSpeed);
			_selectorSize = Mathf.Lerp(_selectorSize, 512*.1f, Time.deltaTime * animSpeed);

			for(int i = 0; i < _l2Offset.Count; i++)
			{
				_l2Offset[i] = Mathf.Lerp(_l2Offset[i], -2, Time.deltaTime * animSpeed);
			}
			if(Mathf.Round(_offset) == -1)
				_menuDisplay = false;
		}


		if(_menuDisplay)
		{
			buttonPos = new Rect(100, -25, 250, 50);
			float rot = startAngle;
			GUI.skin = norm;

			for(int i = 0; i < MenuItemNames.Count; i++)
			{
				rot = startAngle;
				rot += (angleInterval*i);
				rot *= _offset;
				GUIUtility.RotateAroundPivot(rot, _pivot);
				if(GUI.Button(buttonPos, MenuItemNames[i]))
				{
					if(_selectedItem == i)
						_selectedItem = -1;
					else
						_selectedItem = i;
					_windowOpen = false;
				}
				GUIUtility.RotateAroundPivot(-rot, _pivot);
			}
			//Level 2 Buttons
			buttonPos = new Rect(350, -25, 250, 50);
			List<string> names = new List<string>();
			for(int i = 0; i < _l2Offset.Count; i++)
			{
				if(_selectedItem == i)
					_l2Offset[i] = Mathf.Lerp(_l2Offset[i], 1, Time.deltaTime * animSpeed);
				else
					_l2Offset[i] = Mathf.Lerp(_l2Offset[i], -1, Time.deltaTime * animSpeed);
			}

			
			//Play Menu
			names.Clear();
			names.Add("Main Levels");
			names.Add("Custom Levels");
			names.Add("Endless Mode");
			for(int i = 0; i < names.Count; i++)
			{
				rot = startAngle;
				rot += (l2AngleInterval*i);
				rot *= _l2Offset[0];
				GUIUtility.RotateAroundPivot(rot, _pivot);
				if(GUI.Button(buttonPos, names[i]))
				{
					if(i==2)
					{
						//Load Enless Mode
						_windowOpen = false;
					}else
					{
						if(_activeWindow == i)
							_windowOpen = !_windowOpen;
						else
						{
							_windowOpen = true;
							for(int a = 0; a < _windowOffset.Count; a++)
							{
								_windowOffset[a] = 0f;
							}
						}
						_activeWindow = i;
					}
				}
				GUIUtility.RotateAroundPivot(-rot, _pivot);
			}

			//Level Edit Menu
			names.Clear();
			names.Add("New Level");
			names.Add("Edit Level");
			names.Add("Delete Level");
			for(int i = 0; i < names.Count; i++)
			{
				rot = startAngle*3;
				rot += (l2AngleInterval*i);
				rot *= _l2Offset[1];
				GUIUtility.RotateAroundPivot(rot, _pivot);
				if(GUI.Button(buttonPos, names[i]))
				{
					if(_activeWindow == 2+i)
						_windowOpen = !_windowOpen;
					else
					{
						_windowOpen = true;
						for(int a = 0; a < _windowOffset.Count; a++)
						{
							_windowOffset[a] = 0f;
						}
					}
					_activeWindow = 2+i;

				}
				GUIUtility.RotateAroundPivot(-rot, _pivot);
			}

			//Options Menu
			names.Clear();
			names.Add("Gameplay");
			names.Add("Audio");
			names.Add("Video");
			names.Add("Controls");
			for(int i = 0; i < names.Count; i++)
			{
				rot = startAngle*5;
				rot += (l2AngleInterval*i);
				rot *= _l2Offset[2];
				GUIUtility.RotateAroundPivot(rot, _pivot);
				if(GUI.Button(buttonPos, names[i]))
				{
					if(_activeWindow == 5+i)
						_windowOpen = !_windowOpen;
					else
					{
						_windowOpen = true;
						for(int a = 0; a < _windowOffset.Count; a++)
						{
							_windowOffset[a] = 0f;
						}
					}
					_activeWindow = 5+i;
				}
				GUIUtility.RotateAroundPivot(-rot, _pivot);
			}

			//Quit Menu
			names.Clear();
			names.Add("Exit Game");
			for(int i = 0; i < names.Count; i++)
			{
				rot = startAngle*7;
				rot += (l2AngleInterval*i);
				rot *= _l2Offset[3];
				GUIUtility.RotateAroundPivot(rot, _pivot);
				if(GUI.Button(buttonPos, names[i]))
				{
					Application.Quit();
				}
				GUIUtility.RotateAroundPivot(-rot, _pivot);
			}
			GUI.skin = window;
			if(!_windowOpen && _windowDisplay)
			{
				for(int i = 0; i < _windowOffset.Count; i++)
				{
					_windowOffset[i] = Mathf.Lerp(_windowOffset[i], 0, Time.deltaTime * animSpeed);
				}
				if(Mathf.Round(_windowOffset[_activeWindow]) == 0)
					_windowDisplay = false;
			}
			if(_windowOpen && !_windowDisplay)
			{
				for(int i = 0; i < _windowOffset.Count; i++)
				{
					_windowOffset[i] = 0f;
				}
				_windowDisplay = true;
			}
			if(_windowDisplay)
			{
				float halfScreen = (Screen.width/2);
				float winOff;
				switch(_activeWindow)
				{
					//Play
					case 0:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(0, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), PlayLevels, "Main Levels");
						break;
					case 1:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(1, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), PlayLevels, "Custom Levels");
						break;
					//Level Editor
					case 2:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(2, new Rect(halfScreen, 50 - winOff, halfScreen-50, 400f*_windowOffset[_activeWindow]), EditLevels, "New Level");
						break;
					case 3:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(3, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), EditLevels, "Edit Levels");
						break;
					case 4:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(4, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), EditLevels, "Delete Levels");
						break;
					//Options
					case 5:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(5, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), OptionsMenu, "Gameplay");
						break;
					case 6:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(6, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), OptionsMenu, "Audio");
						break;
					case 7:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(7, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), OptionsMenu, "Video");
						break;
					case 8:
						_windowOffset[_activeWindow] = Mathf.Lerp(_windowOffset[_activeWindow], 1, Time.deltaTime * animSpeed);
						winOff = 1000 * (1-_windowOffset[_activeWindow]);
						GUI.Window(8, new Rect(halfScreen, 50 - winOff, halfScreen-50, 512f*_windowOffset[_activeWindow]), OptionsMenu, "Controls");
						break;
				}
			}
			renderSelectorRing();

		}
		//Burst
		if(_isBurt)
		{
			float size = 512*_burstScale;
			Rect burstSize = new Rect(size/-2, size/-2, size, size);
			_burstScale = Mathf.Lerp(_burstScale, burstFinal, Time.deltaTime * (animSpeed/2));
			GUI.DrawTexture(burstSize, burst);
			if(Mathf.Round(_burstScale) == burstFinal)
			{
				_isBurt = false;
				_burstScale = burstInit;
			}
		}
		GUI.skin = skinCore;
		if(_menuOpen)
		{
			_coreOffset = Mathf.Lerp(_coreOffset, -1, Time.deltaTime * animSpeed);
		}
		GUIUtility.RotateAroundPivot(90*_coreOffset, _pivot);
		if(GUI.Button(new Rect(-128,-128, 256, 256), ""))
		{
			_menuOpen = !_menuOpen;
			_burstScale = burstInit;
			_windowOpen = false;
			_isBurt = true;
			_reload = true;
		}
		GUIUtility.RotateAroundPivot(90*-_coreOffset, _pivot);
		GUI.skin = window;
		if(firstLaunch)
		{
			float hw = Screen.width/2;
			float hh = Screen.height/2;
			GUI.Window(-1,new Rect(hw-256, hh-150, 512,300), UsernameWin, "Welcome");
		}

	}
	//Username Window
	void UsernameWin(int windowID)
	{
		float left = 50;
		float top = 50;
		float pad = 5;
		float lh = 64;
		float width = (512)-100;
		GUI.Label(new Rect(left, top+((pad+lh)*0), width, lh), "Player Name:");
		_playerName = GUI.TextField(new Rect(left, top+((pad+lh)*1), width, lh), _playerName);
		if(GUI.Button(new Rect(left, top+((pad+lh)*2), width, lh), "Go"))
		{
			PlayerPrefs.SetInt("first?", 1);
			firstLaunch = false;
			PlayerPrefs.SetString("name", _playerName);
		}
	}
	//Pay Levels Window
	void PlayLevels (int windowID) 
	{
		float left = 40;
		float top = 100;
		float pad = 10;
		float lh = 128;
		float width = (Screen.width/2)-150;
		GUI.skin = list;
		if(windowID == 0)//Main Levels Window
		{
			if(_mainLevels.Count == 0)
			{
				GUI.Label(new Rect(left, top-20, 300, 30), "There are no levels to Display.");
				if(GUI.Button(new Rect(left, top+20, 200, 64), "Reload"))
				{
					_reload = true;
				}
			}else
			{
				_windowScrollPos[0] = GUI.BeginScrollView(new Rect(left, top-30, width+20, 400), _windowScrollPos[0], new Rect(0,0, width, (lh+pad)*_mainLevels.Count));
				for(int i = 0; i < _mainLevels.Count; i++)
				{
					GUI.Box(new Rect(0, ((lh+pad)*i),width, lh), i+"");
					GUI.Label(new Rect(80, ((lh+pad)*i)+20, 200, 30), _mainLevels[i].LevelName);
					GUI.Label(new Rect(80, ((lh+pad)*i)+50, 200, 30), _mainLevels[i].Author);
					GUI.Label(new Rect(80, ((lh+pad)*i)+80, 200, 30), _mainLevels[i].Waves.Count + " Waves");
					if(GUI.Button(new Rect(width-150, (top+((lh+pad)*i))+30,150, 64), "Play"))
					{
						PlayerPrefs.SetString("loadPath",Application.dataPath+"/Levels/Main/" + _mainLevels[i].LevelName + ".lvl");
						Application.LoadLevel("PlayLevel");
					}
				}
				GUI.EndScrollView();
			}
		}else if(windowID == 1)//Custom Levels Window 
		{
			if(_customLevels.Count == 0)
			{
				GUI.Label(new Rect(left, top-20, 300, 30), "There are no levels to Display.");
				if(GUI.Button(new Rect(left, top+20, 200, 64), "Reload"))
				{
					_reload = true;
				}
			}else
			{
				_windowScrollPos[1] = GUI.BeginScrollView(new Rect(left, top-30, width+20, 400), _windowScrollPos[1], new Rect(0,0, width, (lh+pad)*_customLevels.Count));
				for(int i = 0; i < _customLevels.Count; i++)
				{
					GUI.Box(new Rect(0, ((lh+pad)*i),width, lh), i+"");
					GUI.Label(new Rect(80, ((lh+pad)*i)+20, 200, 30), _customLevels[i].LevelName);
					GUI.Label(new Rect(80, ((lh+pad)*i)+50, 200, 30), _customLevels[i].Author);
					GUI.Label(new Rect(80, ((lh+pad)*i)+80, 200, 30), _customLevels[i].Waves.Count + " Waves");
					if(GUI.Button(new Rect(width-150, ((lh+pad)*i)+30,150, 64), "Play"))
					{
						PlayerPrefs.SetString("loadPath",Application.dataPath+"/Levels/Custom/" + _customLevels[i].LevelName + ".lvl");
						Application.LoadLevel("PlayLevel");
					}
				}
				GUI.EndScrollView();
			}
		}
	}
	//Edit Levels Window
	void EditLevels (int windowID) 
	{
		float left = 40;
		float top = 100;
		float pad = 10;
		float lh = 64;
		float width = (Screen.width/2)-150;
		if(windowID == 2)//New Level Window
		{
			GUI.Label(new Rect(left, top+((pad+lh)*0), width, lh), "Level Name:");
			_newLevelName = GUI.TextField(new Rect(left, top+((pad+lh)*1), width, lh), _newLevelName);
			if(GUI.Button(new Rect(left, top+((pad+lh)*2), width, lh), "Create"))
			{
				PlayerPrefs.SetString("creating", _newLevelName);
				Application.LoadLevel("LevelEditor");
			}
		}else if(windowID == 3)//Load Level Window
		{
			lh = 128;
			GUI.skin = list;
			if(_customLevels.Count == 0)
			{
				GUI.Label(new Rect(left, top-20, 300, 30), "There are no levels to Display.");
				if(GUI.Button(new Rect(left, top+20, 200, 64), "Reload"))
				{
					_reload = true;
				}
			}else
			{
				_windowScrollPos[2] = GUI.BeginScrollView(new Rect(left, top-30, width+20, 400), _windowScrollPos[2], new Rect(0,0, width, (lh+pad)*_customLevels.Count));
				for(int i = 0; i < _customLevels.Count; i++)
				{
					GUI.Box(new Rect(0, ((lh+pad)*i),width, lh), i+"");
					GUI.Label(new Rect(80, ((lh+pad)*i)+20, 200, 30), _customLevels[i].LevelName);
					GUI.Label(new Rect(80, ((lh+pad)*i)+50, 200, 30), _customLevels[i].Author);
					GUI.Label(new Rect(80, ((lh+pad)*i)+80, 200, 30), _customLevels[i].Waves.Count + " Waves");
					if(GUI.Button(new Rect(width-150, ((lh+pad)*i)+30,150, 64), "Edit"))
					{
						PlayerPrefs.SetString("editorMode", "load");
						PlayerPrefs.SetString("loadPath", Application.dataPath+"/Levels/Custom/"+_customLevels[i].LevelName+".lvl");
						Application.LoadLevel("LevelEditor");
					}
				}
				GUI.EndScrollView();
			}
		}else if(windowID == 4)
		{
			lh = 128;
			GUI.skin = list;
			if(_customLevels.Count == 0)
			{
				GUI.Label(new Rect(left, top-20, 300, 30), "There are no levels to Display.");
				if(GUI.Button(new Rect(left, top+20, 200, 64), "Reload"))
				{
					_reload = true;
				}
			}else
			{
				_windowScrollPos[3] = GUI.BeginScrollView(new Rect(left, top-30, width+20, 400), _windowScrollPos[3], new Rect(0,0, width, (lh+pad)*_customLevels.Count));
				for(int i = 0; i < _customLevels.Count; i++)
				{
					GUI.Box(new Rect(0, ((lh+pad)*i),width, lh), i+"");
					GUI.Label(new Rect(80, ((lh+pad)*i)+20, 200, 30), _customLevels[i].LevelName);
					GUI.Label(new Rect(80, ((lh+pad)*i)+50, 200, 30), _customLevels[i].Author);
					GUI.Label(new Rect(80, ((lh+pad)*i)+80, 200, 30), _customLevels[i].Waves.Count + " Waves");
					if(GUI.Button(new Rect(width-150, ((lh+pad)*i)+30,150, 64), "Delete"))
					{
						DeleteLevel(i);
					}
				}
				GUI.EndScrollView();
			}
		}
	}
	//Options Menu Window
	void OptionsMenu (int windowID) 
	{
		float left = 40;
		float top = 80;
		float pad = 5;
		float lh = 40;
		float width = (Screen.width/2)-150;
		if(windowID == 5)//Gameplay
		{
			GUI.skin = list;
			_windowScrollPos[4] = GUI.BeginScrollView(new Rect(left, top, width, 500), _windowScrollPos[4], new Rect(0,0, width, 500));
			GUI.Label(new Rect(0, (lh + pad)*0, 500, lh), "Player Name");
			_playerName = GUI.TextField(new Rect(0, (lh + pad)*1, 250, lh), _playerName);
			_mouseMode = GUI.Toggle(new Rect(0, (lh + pad)*2, 250, lh), _mouseMode, "Mouse Mode");
			GUI.EndScrollView();
			if(GUI.Button(new Rect(left, 410, width/2, 64), "Apply"))
			{
				PlayerPrefs.SetString("name", _playerName);
				_data.RegisterData("MouseMode", _mouseMode);
				SaveSettings();
			}

			if(GUI.Button(new Rect((width/2)+left+pad, 410, width/2, 64), "Revert"))
			{
				LoadSettings();
				_playerName = PlayerPrefs.GetString("name");
			}
		}
		if(windowID == 6)//Audio
		{
			lh = 40;
			GUI.skin = list;
			_windowScrollPos[4] = GUI.BeginScrollView(new Rect(left, top, width, 500), _windowScrollPos[4], new Rect(0,0, width, 500));
			GUI.Label(new Rect(0, (lh + pad)*0, 500, lh), "Master Volume - " + Round(_masterVol, 100));
			_masterVol = GUI.HorizontalSlider(new Rect(0, (lh + pad)*1, 250, lh), _masterVol, 0, 100);
			GUI.Label(new Rect(0, (lh + pad)*2, 500, lh), "Master Volume - " + Round(_bgmVol, 100));
			_bgmVol = GUI.HorizontalSlider(new Rect(0, (lh + pad)*3, 250, lh), _bgmVol, 0, 100);
			GUI.Label(new Rect(0, (lh + pad)*4, 500, lh), "Master Volume - " + Round(_sfxVol, 100));
			_sfxVol = GUI.HorizontalSlider(new Rect(0, (lh + pad)*5, 250, lh), _sfxVol, 0, 100);
			GUI.EndScrollView();
			if(GUI.Button(new Rect(left, 410, width/2, 64), "Apply"))
			{
				_data.RegisterData("BGM_Vol", _bgmVol);
				_data.RegisterData("SFX_Vol", _sfxVol);
				_data.RegisterData("Master_Vol", _masterVol);
				SaveSettings();
			}
			if(GUI.Button(new Rect((width/2)+left+pad, 410, width/2, 64), "Revert"))
			{
				LoadSettings();
				_bgmVol = _data.GetFloat("BGM_Vol");
				_sfxVol = _data.GetFloat("SFX_Vol");
				_masterVol = _data.GetFloat("Master_Vol");
			}

		}
		if(windowID == 7)//Video
		{
			GUI.skin = list;
			if(_resolutions == null)
				_resolutions = Screen.resolutions;
			_windowScrollPos[4] = GUI.BeginScrollView(new Rect(left, top, width, 500), _windowScrollPos[4], new Rect(0,0, width, 500));
			_vSync = GUI.Toggle(new Rect(0, (lh + pad)*0, 250, lh), _vSync, "VSync");
			GUI.Label(new Rect(0, (lh + pad)*1f, 250, lh), "Resolution");
			if(GUI.Button(new Rect(0, ((lh + pad)*1.5f), 350, 64), _resolutions[_resIndex].width + "x" + _resolutions[_resIndex].height + " " + _resolutions[_resIndex].refreshRate + "Hz"))
			{
				_resIndex++;
				if(_resIndex > _resolutions.Length-1)
				{
					_resIndex = 0;
				}
			}
			_fullscreen = GUI.Toggle(new Rect(0, (lh + pad)*3f, 400, lh), _fullscreen, "Fullscreen");
			GUI.EndScrollView();
			if(GUI.Button(new Rect(left, 410, width/3, 64), "Apply"))
			{
				_data.RegisterData("VSync", _vSync);
				_data.RegisterData("Res_Width", _resolutions[_resIndex].width);
				_resWidth = _resolutions[_resIndex].width;
				_data.RegisterData("Res_Height", _resolutions[_resIndex].height);
				_resHeight = _resolutions[_resIndex].height;
				_data.RegisterData("Res_Refresh", _resolutions[_resIndex].refreshRate);
				_resRefresh = _resolutions[_resIndex].refreshRate;
				_data.RegisterData("Fullscreen", _fullscreen);
				SaveSettings();
			}
			if(GUI.Button(new Rect((width/2)+left+pad, 410, width/2, 64), "Revert"))
			{
				LoadSettings();
				//_vSync = _data.GetBool("VSync");
			}
			
		}
	}
	void SaveSettings()
	{
		_data.SaveData();
		_controls.SaveData();
		ApplySettings();
	}

	void ApplySettings()
	{
		Screen.SetResolution(_resWidth, _resHeight, _fullscreen, _resRefresh);
		if(_vSync)
			QualitySettings.vSyncCount = 1;
		else
			QualitySettings.vSyncCount = 0;
	}

	float Round(float n, float d)
	{
		return Mathf.Round(n*d)/d;
	}
	void renderSelectorRing()
	{
		float desR = startAngle+(angleInterval*_selectedItem);
		if(_menuOpen)
		{
			_selectorPos = Mathf.Lerp(_selectorPos, desR, Time.deltaTime * animSpeed);
			_selectorSize = Mathf.Lerp(_selectorSize, 512*1.44f, Time.deltaTime * animSpeed);
		}
		GUIUtility.RotateAroundPivot(_selectorPos, _pivot);

		GUI.DrawTexture(new Rect(_selectorSize/-2,_selectorSize/-2,_selectorSize,_selectorSize), selctorRing);
		GUIUtility.RotateAroundPivot(-_selectorPos, _pivot);
	}
}
