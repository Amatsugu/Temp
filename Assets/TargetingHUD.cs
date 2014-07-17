using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingHUD : MonoBehaviour {
	public int size = 100;
	public Color color;
	public float res = .5f;
	public bool reset = false;
	public Vector2 offset;

	private GameObject[] enemies;
	private Texture2D tex;
	private List<Rect> rects = new List<Rect>();


	void OnGUI () 
	{
		if(tex == null)
		{
			tex = new Texture2D((int)(Screen.width*res), (int)(Screen.height*res));
			tex.filterMode = FilterMode.Point;
		}
		if(rects == null)
			rects = new List<Rect>();
		if(reset)
		{
			tex = new Texture2D((int)(Screen.width*res), (int)(Screen.height*res));
			tex.filterMode = FilterMode.Point;
			rects = new List<Rect>();
			reset = false;
		}
		enemies = GameObject.FindGameObjectsWithTag("enemy");

		rects.Clear();
		foreach(GameObject g in enemies)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint(g.transform.position);
			pos.x -= (int)((size/2)+offset.x);
			pos.y -= (int)((size/2)+offset.y);
			rects.Add(new Rect((int)(pos.x*res), (int)(pos.y*res), (int)(size*res), (int)(size*res)));
		}
		if(rects.Count == 0)
			return;
		for(int y = 0; y < (int)(Screen.height*res); y++)
		{
			for(int x = 0; x < (int)(Screen.width*res); x++)
			{
				if(tex.GetPixel(x,y) != Color.clear)
					tex.SetPixel(x,y,Color.clear);
				foreach(Rect r in rects)
				{
					if(r.Contains(new Vector3(x,y,0)))
					{
						if(x == r.x || x+1 == r.x+r.width)
							tex.SetPixel(x,y,color);
						if(y == r.y || y+1 == r.y+r.height)
							tex.SetPixel(x,y,color);
					}
				}
			}
		}
		tex.Apply();
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), tex);
	}
}
