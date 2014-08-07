using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour {

	public GameObject pointer;
	public Camera cam;
	public List<GameObject> units = new List<GameObject>();
	public GUISkin window;
	public GUISkin norm;
	public GUISkin list;
	public float animSpeed = 3f;

	private bool _unitsWindow = false;
	private float _unitWinPos = 5f;
	private GameObject _curObject;
	private bool _updatePointer = true;
	private Level _currentLevel = new Level();
	private Vector3 _desPoint;
	private int _selectedUnit = -1;
	private int _curWave = 0;
	private UnitStats _unitStats;
	private bool _hasChanged = false;
	private Wave _thisWave;
	private UnitStats _objectToReset;
	private List<GameObject> _feildUnits = new List<GameObject>();
	private bool _waveMode = false;
	private Vector2 _unitScrollPos;
	private float _editorWindowH = 320;
	private Vector2 _waveScrollPos;
	private int _waveToDelete = -1;
	private bool _isSaving = false;
	private ControlMap _controls;
	// Use this for initialization
	void Start () 
	{
		_controls = GameObject.Find("_GameRegistry").GetComponent<ControlMap>();
		if(PlayerPrefs.GetString("editorMode") == "load")
		{
			if(_currentLevel.Load(PlayerPrefs.GetString("loadPath")))
			{
				_currentLevel.LevelName = PlayerPrefs.GetString("creating");
				_currentLevel.Author = PlayerPrefs.GetString("name");
				CreateWave();
			}
		}else
		{
			_currentLevel.LevelName = PlayerPrefs.GetString("creating");
			_currentLevel.Author = PlayerPrefs.GetString("name");
			CreateWave();
		}

	}
	void CreateWave()
	{
		_currentLevel.Waves.Add(new Wave());
		_curWave = _currentLevel.Waves.Count-1;
		_currentLevel.Waves[_curWave].WaveName = "Wave " + _curWave;
		_thisWave = _currentLevel.Waves[_curWave];
		_hasChanged = true;
	}
	// Update is called once per frame
	void Update () 
	{
		if(_controls.GetKeyDown("Toggle Editor"))
		{
			_unitsWindow = !_unitsWindow;
			if(_unitsWindow)
				_updatePointer = false;
			else
				_updatePointer = true;
		}
		if(_controls.GetKeyDown("Cancel"))
		{
			_selectedUnit = -1;
		}

		if(_updatePointer)
		{
			Vector3 mousePos = Input.mousePosition;
			RaycastHit hit;
			Physics.Raycast(cam.ScreenPointToRay(mousePos), out hit);
			_desPoint = new Vector3(Mathf.Round(hit.point.x),Mathf.Round(hit.point.y),0);
			if(_desPoint.x > 10)
				_desPoint.x = 10;
			if(_desPoint.x < -10)
			   _desPoint.x = -10;
			if(_desPoint.y > 7)
				_desPoint.y = 7;
			if(_desPoint.y < -7)
			   _desPoint.y = -7;
			pointer.transform.position = _desPoint;
		}
		Vector2 v2Pos = new Vector2(_desPoint.x, _desPoint.y);
		//Update Field
		if(_hasChanged)
		{
			foreach(GameObject g in _feildUnits)
			{
				Destroy(g);
			}
			_feildUnits.Clear();
			for(int i = 0; i < _currentLevel.Waves[_curWave].UnitID.Count; i++)
			{
				Wave thisWave = _currentLevel.Waves[_curWave];
				Vector3 newPos = new Vector3(thisWave.UnitPos[i].x,thisWave.UnitPos[i].y,0);
				GameObject newUnit = Instantiate(units[thisWave.UnitID[i]], newPos, Quaternion.identity) as GameObject;
				_feildUnits.Add(newUnit);
			}
			_hasChanged = false;
		}
		if(_selectedUnit != -1)
		{
			if((_curObject == null))
			{
				_curObject = Instantiate(units[_selectedUnit], _desPoint, Quaternion.identity) as GameObject;
				_curObject.GetComponent<UnitStats>().SetGreen();
				_unitStats = _curObject.GetComponent<UnitStats>();
			}else if(_unitStats.unitID != units[_selectedUnit].GetComponent<UnitStats>().unitID)
			{
				Destroy(_curObject);
				_curObject = Instantiate(units[_selectedUnit], _desPoint, Quaternion.identity) as GameObject;
				_curObject.GetComponent<UnitStats>().SetGreen();
				_unitStats = _curObject.GetComponent<UnitStats>();
			}else
			{
				_curObject.transform.position = _desPoint;
			}
			if(!_thisWave.UnitPos.Contains(v2Pos))
			{
				_unitStats.SetGreen();
				if(Input.GetKeyDown(KeyCode.Mouse0) && !_unitsWindow)
				{
					_thisWave.UnitID.Add(_unitStats.unitID);
					_thisWave.UnitPos.Add(v2Pos);
					_hasChanged = true;
				}
			}else
			{
				_unitStats.SetRed();
			}
		}else
		{
			if(_updatePointer)
			{
				if(_thisWave.UnitPos.Contains(v2Pos))
				{
					if(_objectToReset != null)
						_objectToReset.SetNone();
					_objectToReset = GetFieldObj(_desPoint).GetComponent<UnitStats>();
					_objectToReset.SetRed();
					if(Input.GetKeyDown(KeyCode.Mouse0))
					{
						int index = _thisWave.UnitPos.IndexOf(v2Pos);
						_thisWave.UnitPos.RemoveAt(index);
						_thisWave.UnitID.RemoveAt(index);
						_hasChanged = true;
						_objectToReset = null;
					}
				}else if(_objectToReset != null)
				{
					_objectToReset.SetNone();
					_objectToReset = null;
				}

				Destroy(_curObject);
				_curObject = null;
			}
		}
		if(_unitsWindow)
		{
			_unitWinPos = Mathf.Lerp(_unitWinPos, 5, Time.deltaTime * animSpeed);
		}else
		{
			_unitWinPos = Mathf.Lerp(_unitWinPos, (_editorWindowH-64)*-1, Time.deltaTime * animSpeed);
		}
	}
	GameObject GetFieldObj(Vector3 pos)
	{
		GameObject obj = null;
		foreach(GameObject g in _feildUnits)
		{
			if(g.transform.position == pos)
			{
				obj = g;
			}
		}
		return obj;
	}
	void OnGUI()
	{
		GUI.skin = window;
		GUI.Window(0, new Rect(5,_unitWinPos, Screen.width-10, _editorWindowH), Editor, "Level Editor");
		GUI.skin  = norm;
		GUI.Label(new Rect(5, Screen.height-30, 500, 30), "Editing: "+_currentLevel.LevelName);
		GUI.Label(new Rect(5, Screen.height-55, 500, 30), "X: "+_desPoint.x + " Y: " + _desPoint.y);
		GUI.Label(new Rect(5, Screen.height-80, 500, 30), "Wave: "+_thisWave.WaveName);
		GUI.skin = window;
		if(GUI.Button(new Rect(new Rect(Screen.width-200, Screen.height-50, 200,60)), "Save"))
		{
			_isSaving = true;
			_updatePointer = false;
		}

		if(_isSaving)
		{
			GUI.Window(1, new Rect((Screen.width/2)-256, (Screen.height/2)-200, 512,300), SaveWindow, "Save");
		}
	}
	void SaveWindow(int ID)
	{
		float left = 50;
		float top = 50;
		float pad = -10;
		float lh = 55;
		float width = 412;
		GUI.Label(new Rect(left, top+((pad+lh)*0), width, lh), "Level Name:");
		_currentLevel.LevelName = GUI.TextField(new Rect(left, top+((pad+lh)*1), width, lh), _currentLevel.LevelName);
		if(GUI.Button(new Rect(left, top+((pad+lh)*2), width, lh), "Save"))
		{
			_currentLevel.Save();
			_isSaving = false;
			_updatePointer = true;
		}
		if(GUI.Button(new Rect(left, top+((pad+lh)*3), width, lh), "Save & Exit"))
		{
			_currentLevel.Save();
			Application.LoadLevel(0);
		}
		if(GUI.Button(new Rect(left, top+((pad+lh)*4), width, lh), "Exit"))
		{
			Application.LoadLevel(0);
		}
	}
	
	void Editor(int ID)
	{
		float width = (Screen.width-60);
		float left = 35;
		float top = 175;
		float pad = 5;
		float lh = 25;
		if(!_waveMode)
		{
			_unitScrollPos = GUI.BeginScrollView(new Rect(left, 50, width, 150), _unitScrollPos, new Rect(0, 0, ((128+5)*units.Count) + 128,128));
			for(int i = 0; i < units.Count; i++)
			{
				if(GUI.Button(new Rect(((128+5)*i), 0, 128,128), units[i].GetComponent<UnitStats>().unitName))
				{
					if(_selectedUnit == i)
						_selectedUnit = -1;
					else
						_selectedUnit = i;
				}
			}
			GUI.EndScrollView();
			GUI.skin = norm;
			if(_selectedUnit != -1)
			{
				UnitStats curUnit = units[_selectedUnit].GetComponent<UnitStats>();
				GUI.Label(new Rect(left, top+((pad+lh)*0), 200, lh), "Unit: " + curUnit.unitName);
				GUI.Label(new Rect(left + 200, top+((pad+lh)*0), 200, lh), "Max Health: " + curUnit.health);
				GUI.Label(new Rect(left, top+((pad+lh)*1), 200, lh), "Damage: " + curUnit.damageOutput);
				GUI.Label(new Rect(left, top+((pad+lh)*2), 200, lh), "Fire Rate: " + curUnit.fireRate);
			}
			GUI.skin = window;
			if(GUI.Button(new Rect(width-180, 20, 200, 64), "Wave Mode"))
			{
				_waveMode = true;
			}
		}else
		{
			if(GUI.Button(new Rect(width-180, 20, 200, 64), "Unit Mode"))
			{
				_waveMode = false;
			}
			lh = 128;
			width *= .75f;
			_waveScrollPos = GUI.BeginScrollView(new Rect(left, 50, width, _editorWindowH-100), _waveScrollPos, new Rect(0,0, width-50, ((lh+pad)*_currentLevel.Waves.Count)));
			GUI.skin = list;
			width -= 50;
			for(int i = 0; i < _currentLevel.Waves.Count; i++)
			{
				GUI.Box(new Rect(0,((lh+pad)*i), width, lh), i+"");
				if(_curWave == i)
					GUI.enabled = false;
				if(GUI.Button(new Rect( width - 200, ((lh/2)-25)+((lh+pad)*i), 150, 50), "Select"))
				{
					_curWave = i;
					_thisWave = _currentLevel.Waves[_curWave];
					_hasChanged = true;
				}
				if(_currentLevel.Waves.Count <= 1)
				{
					GUI.enabled = false;
				}
				if(GUI.Button(new Rect( width - 350, ((lh/2)-25)+((lh+pad)*i), 150, 50), "Delete"))
				{
					_waveToDelete = i;
				}
				GUI.enabled = true;
				_currentLevel.Waves[i].WaveName = GUI.TextField(new Rect(50, ((lh/2)-25)+((lh+pad)*i), 300,50), _currentLevel.Waves[i].WaveName);
			}
			GUI.EndScrollView();
			width = Screen.width-60;
			if(GUI.Button(new Rect(width-180, 89, 200, 64), "Add Wave"))
			{
				CreateWave();
			}
		}
		if(_waveToDelete != -1)
		{
			_currentLevel.Waves.RemoveAt(_waveToDelete);
			if(_curWave >= _waveToDelete)
			{
				_curWave += 1;
				if(_curWave > _currentLevel.Waves.Count-1)
				{
					_curWave = _currentLevel.Waves.Count-1;
				}
				_thisWave = _currentLevel.Waves[_curWave];
				_hasChanged = true;
			}
			_waveToDelete = -1;
		}
		if(_waveMode)
		{
			_editorWindowH = Mathf.Lerp(_editorWindowH, 600, Time.deltaTime * animSpeed);
		}else
		{
			_editorWindowH = Mathf.Lerp(_editorWindowH, 320, Time.deltaTime * animSpeed);
		}
		GUI.skin = window;
		if(GUI.Button(new Rect(25, _editorWindowH-64, width, 64), "Hide/Show Editor Window"))
		{
			_unitsWindow = !_unitsWindow;
			if(_unitsWindow)
				_updatePointer = false;
			else
				_updatePointer = true;
		}
	}
}
