using UnityEngine;
using System.Collections;

public class UnitStats : MonoBehaviour {
	
	public string unitName = "";
	public float damageOutput;
	public float fireRate;
	public float health = 100;
	public int unitID;

	public GameObject _greenSel;
	public GameObject _redSel;

	public void SetRed()
	{
		_greenSel.SetActive(false);
		_redSel.SetActive(true);
	}
	public void SetGreen()
	{
		_greenSel.SetActive(true);
		_redSel.SetActive(false);
	}
	public void SetNone()
	{
		_greenSel.SetActive(false);
		_redSel.SetActive(false);
	}
}
