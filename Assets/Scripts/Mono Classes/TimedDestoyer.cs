using UnityEngine;
using System.Collections;

public class TimedDestoyer : MonoBehaviour {

	public float lifeTime;

	private float _destroyTime;
	private float _pauseTime;

	void Start () 
	{
		_destroyTime = Time.time + lifeTime;
	}

	void Update () 
	{
		if(Time.time >= _destroyTime)
			Destroy(gameObject);
	}

	public void Pause()
	{
		_pauseTime = Time.time;
	}
	
	public void UnPause()
	{
		_destroyTime += (Time.time- _pauseTime);
	}
}
