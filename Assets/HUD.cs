using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	public class Bound
	{
		public Vector2 upperBound;
		public Vector2 lowerBound;

		public bool Contains(Vector2 point)
		{
			if((point.x > lowerBound.x-1 && upperBound.x+1 > point.x) && (point.y > lowerBound.y-1 && upperBound.y+1 > point.y))
			{
				return true;
			}else
			{
				return false;
			}
		}
	}
	public List<Bound> objects = new List<Bound>();
	public Color color = new Color();
	public Texture2D tex;

	void Start()
	{
		tex = new Texture2D(Screen.width, Screen.height);
		tex.wrapMode = TextureWrapMode.Clamp;
		List<Color> col = new List<Color>();
		for(int x = 0; x < Screen.width; x++)
		{
			for(int y = 0; y < Screen.height; y++)
			{
				col.Add(new Color(0,0,0,0));
			}
		}
		tex.SetPixels(0,0,Screen.width,Screen.height ,col.ToArray());
		tex.Apply();

	}
	
	void OnGUI () 
	{
		GameObject[] tar = GameObject.FindGameObjectsWithTag("enemy");
		objects.Clear();
		foreach(GameObject g in tar)
		{
			objects.Add(new Bound());
			int i = objects.Count-1;
			Vector3 p = Camera.main.WorldToScreenPoint(g.transform.position);

			objects[i].upperBound = new Vector2(p.x + 20, p.y + 20);
			objects[i].lowerBound = new Vector2(p.x - 20, p.y - 20);
		}
		foreach(Bound b in objects)
		{
			//Debug.Log(b.lowerBound + "|" + b.upperBound);
			for(int x = (int)b.lowerBound.x; x <= (int)b.upperBound.x; x++)
			{
				for(int y = (int)b.lowerBound.y; y <= (int)b.upperBound.y; y++)
				{
					if(TestOverlap(new Vector2(x,y)))
					{
						tex.SetPixel(x,y,color);
					}
				}
			}
		}
		tex.Apply();

		GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), tex);
	}

	bool TestOverlap(Vector2 point)
	{
		bool ret = false;
		foreach(Bound b in objects)
		{
			if(b.Contains(point))
			{
				ret = true;
				break;
			}
		}
		return ret;
	}
}
