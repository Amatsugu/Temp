﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level 
{
	public string LevelName = "";
	public List<Wave> Waves = new List<Wave>();
	public string Author = "";

	public string[] GetLevelData()
	{
		List<string> levelData = new List<string>();
		levelData.Add("LevelName:"+LevelName);
		levelData.Add("Author:"+Author);
		foreach(Wave w in Waves)
		{
			string waveStr = w.WaveName;
			for(int i = 0; i < w.UnitID.Count; i++)
			{
				waveStr += "," + w.UnitID[i] + ":" + w.UnitPos[i].x + ":" + w.UnitPos[i].y;
			}
			levelData.Add(waveStr);
		}
		return levelData.ToArray();
	}

	public bool SetLevelData(string[] levelData)
	{
		if(!levelData[0].Contains("LevelName:"))
		{
			return false;
		}else
		{
			LevelName = levelData[0].Split(':')[1];
			Author = levelData[1].Split(':')[1];
			for(int i = 2; i < levelData.Length; i++)
			{
				Wave w = new Wave();
				string[] units = levelData[i].Split(',');
				w.WaveName = units[0];
				for(int d = 1; d < units.Length; d++)
				{
					string[] unitData = units[d].Split(':');
					w.UnitID.Add(int.Parse(unitData[0]));
					w.UnitPos.Add(new Vector2(float.Parse(unitData[1]),float.Parse(unitData[2])));
				}
				Waves.Add(w);
			}
			return true;
		}
	}
}
