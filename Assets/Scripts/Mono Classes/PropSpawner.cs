using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropSpawner : MonoBehaviour {

	public float upperLimit = 7;
	public float lowerLimit = -7;
	public float spawnRate = 5;
	public List<GameObject> props = new List<GameObject>();

	private float _nextSpawnTime;
	private float _pauseTime;

	void Start () 
	{
		_nextSpawnTime = Time.time + spawnRate;
	}
	
	void Update () 
	{
		if(Time.time >= _nextSpawnTime)
		{
			SpawnProp();
		}
	}

	void SpawnProp()
	{
		Vector3 spawnPos = new Vector3(transform.position.x, Random.Range(lowerLimit, upperLimit), transform.position.z + Random.Range(lowerLimit/2, upperLimit/2));
		Instantiate(props[Random.Range(0, props.Count)], spawnPos, Random.rotation);
		_nextSpawnTime = Time.time + spawnRate;
	}

	public void Pause()
	{
		_pauseTime = Time.time;
	}
	
	public void UnPause()
	{
		_nextSpawnTime += (Time.time- _pauseTime);
	}
}
