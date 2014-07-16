using UnityEngine;
using System.Collections;

public class UnitStats : MonoBehaviour {
	
	public string unitName = "";
	public float damageOutput;
	public float fireRate;
	public float health = 100;
	public int unitID;

	public void SetRed()
	{
		renderer.materials[1].SetColor("_OutlineColor", Color.red);
	}
	public void SetGreen()
	{
		renderer.materials[1].SetColor("_OutlineColor", Color.green);
	}
	public void SetNone()
	{
		renderer.materials[1].SetColor("_OutlineColor", Color.clear);
	}
}
