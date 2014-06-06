using UnityEngine;
using System.IO;
using System.Collections;

public class DefaultData : MonoBehaviour{


	void Start()
	{
		if(GameObject.Find("_GameRegistry") != gameObject)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		LoadAllCFGs();
	}

	public void LoadAllCFGs()
	{
		ControlMap cMap = gameObject.GetComponent<ControlMap>();
		DataMap dMap = gameObject.GetComponent<DataMap>();
		if(File.Exists(Application.dataPath+"/data.cfg"))
		{
			dMap.LoadData();
			Debug.Log("Loaded Data File");
		}else
			dMap = RegisterDefaultData(dMap);
		if(File.Exists(Application.dataPath+"/controls.cfg"))
		{
			cMap.LoadData();
			Debug.Log("Loaded Controls File");
		}else
			cMap = RegisterDefaultControls(cMap);
	}

	public ControlMap RegisterDefaultControls(ControlMap _Controls)
	{
		//Debug.Log("Resetting Controls");
		_Controls.Clear();
		_Controls.RegisterKey("Ability", KeyCode.Space);
		_Controls.RegisterKey("Up", KeyCode.W);
		_Controls.RegisterKey("Down", KeyCode.S);
		_Controls.RegisterKey("Left", KeyCode.A);
		_Controls.RegisterKey("Right", KeyCode.D);
		_Controls.RegisterKey("Percise Move", KeyCode.LeftShift);
		_Controls.RegisterKey("Pause", KeyCode.Escape);

		_Controls.RegisterKey("Cancel", KeyCode.Escape);
		_Controls.RegisterKey("Toggle Editor", KeyCode.E);
		_Controls.RegisterKey("Advance", KeyCode.Space);
		return _Controls;
	}
	public DataMap RegisterDefaultData(DataMap _DataMap)
	{
		//Debug.Log("Resetting Data");
		_DataMap.Clear();
		//Gameplay
		_DataMap.RegisterData("MouseMode", true);

		//Video
		_DataMap.RegisterData("VSync", false);
		_DataMap.RegisterData("PixelLights", 2);
		_DataMap.RegisterData("Tex_Quality", 0);
		_DataMap.RegisterData("Res_Width", Screen.width);
		_DataMap.RegisterData("Res_Height", Screen.height);
		_DataMap.RegisterData("Res_Refresh", Screen.currentResolution.refreshRate);
		_DataMap.RegisterData("Anti_Aliasing", QualitySettings.antiAliasing);
		_DataMap.RegisterData("Aniso_Filtering", false);
		_DataMap.RegisterData("Fullscreen", false);

		//Audio
		_DataMap.RegisterData("SFX_Vol", 100f);
		_DataMap.RegisterData("BGM_Vol", 100f);
		_DataMap.RegisterData("Master_Vol", 100f);
		return _DataMap;
	}
}
