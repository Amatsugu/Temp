using UnityEngine;
using System.Collections;

public class PowerUpObject : MonoBehaviour {
	
	public float value;
	public PowerUp type;

	private Transform _player;
	private bool _isPaused;

	void Start () 
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update () 
	{
		if(_player != null)
		{
			if(!_isPaused)
				transform.position = Vector3.Lerp(transform.position, _player.position, Time.deltaTime);
			if(Vector3.Distance(transform.position, _player.position) < 1)
				Apply();
		}
	}

	void Apply()
	{
		_player.GetComponent<PlayerController>().AddPower(type, value);
		Destroy(gameObject);
	}

	public void Pause()
	{
		_isPaused = true;
	}
	
	public void UnPause()
	{
		_isPaused = false;
	}
	
}
