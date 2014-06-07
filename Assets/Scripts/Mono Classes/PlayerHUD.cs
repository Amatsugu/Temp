using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour {

	// Use this for initialization
	public Vector2 hudOffset;
	public Texture2D hudFrame;
	public Texture2D healthBar;
	public Texture2D selector;
	public float hudScale = 1;
	public GUISkin skin;

	private PlayerController _pc;
	//432 55
	//198x198 95 57

	void Start () 
	{
		_pc = GetComponent<PlayerController>();
	}
	
	void OnGUI()
	{
		if(_pc == null)
			_pc = GetComponent<PlayerController>();
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		pos.y = Screen.height-pos.y;
		float hW = hudFrame.width*hudScale;
		float hH = hudFrame.height*hudScale;
		GUI.BeginGroup(new Rect(pos.x-hW+hudOffset.x, pos.y-(hH/2)+hudOffset.y, hW, hH));
		GUI.DrawTexture(new Rect(0, 0, hW, hH), hudFrame);
		float pHealth = (_pc.GetHealth()/_pc.maxHealth);
		float hBarHeight = (healthBar.height*hudScale)*pHealth;
		GUI.DrawTexture(new Rect(432*hudScale, (55*hudScale)+((healthBar.height*hudScale)-hBarHeight), healthBar.width*hudScale,hBarHeight), healthBar);
		List<PowerUp> powers = _pc.GetPowers();
		float sH = selector.height;
		int curSel = _pc.GetSelectedPower();
		GUI.DrawTexture(new Rect((95-16) * hudScale, (57 + (38*curSel) + (198 * curSel) - 16)*hudScale, selector.width * hudScale, sH * hudScale), selector);
		for(int i = 0; i < powers.Count; i++)
		{
			GUI.Box(new Rect(95 * hudScale, (57 + (38*i) + (198 * i))*hudScale, 198 * hudScale, 198 * hudScale), powers[i].ToString());
		}
		GUI.EndGroup();
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(pos.x+hudOffset.x, pos.y-(hH/2)+hudOffset.y, hH, hW));
		GUIUtility.RotateAroundPivot(90, new Vector2(0,0));
		if(curSel < powers.Count)
			GUI.Label(new Rect(67*hudScale, 230*hudScale, 250, 25), powers[curSel].ToString());
		GUI.Label(new Rect(771*hudScale, 190*hudScale, 250, 25), (pHealth*100)+"%");
		GUI.EndGroup();
	}
}
