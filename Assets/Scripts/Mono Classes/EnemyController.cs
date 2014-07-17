using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	public float health;
	public float moveSpeed = 5;
	public float bulletSpeed = 1000;
	public List<Transform> bulletSpawns = new List<Transform>();
	public bool useGunner = false;
	public Vector2 upperBound;
	public Vector2 lowerBound;
	public GameObject explosion;
	public List<GameObject> powerUps;
	public int dropChance = 50;
	public int healthChance = 5;
	public int maxHealthDrop = 75;
	public float minDuration = 2;
	public float maxDuration = 10;
	
	private Vector3 _curPosition;
	private Vector2 _targetPostion;
	private float _nextFireTime;
	private float _pauseTime;
	private ObjectPoolerWorld _bulletPool;
	private UnitStats _unitStats;

	void Start () 
	{
		if(Application.loadedLevelName == "LevelEditor")
		{
			this.enabled = false;
			if(useGunner)
				GetComponent<Gunner>().enabled = false;
			return;
		}
		if(renderer.materials[1])
			renderer.materials[1] = null;
		_unitStats = GetComponent<UnitStats>();
		health = _unitStats.health;
		_bulletPool = GameObject.Find("_EnemyBulletPool").GetComponent<ObjectPoolerWorld>();
		_nextFireTime = Time.time + _unitStats.fireRate;
	}
	
	void Update () 
	{
		//Move
		if(Mathf.Round(_curPosition.magnitude) == Mathf.Round(_targetPostion.magnitude))
			FindNextPosition();
		_curPosition = Vector3.Lerp(transform.position, new Vector3(_targetPostion.x, _targetPostion.y, 0), Time.deltaTime);
		//Gun
		if(!useGunner)
		{
			if(Time.time >= _nextFireTime)
			{
				Fire();
				_nextFireTime = Time.time + _unitStats.fireRate;
			}
		}
		transform.position = _curPosition;
	}

	void FindNextPosition()
	{
		_targetPostion = new Vector2(Random.Range(lowerBound.x, upperBound.x), Random.Range(lowerBound.y, upperBound.y));
	}

	public void DealDamage(float d)
	{
		health -= d;
		if(health <= 0)
			Die();
	}

	void Die()
	{
		Instantiate(explosion, transform.position, Quaternion.identity);
		if(Mathf.Round(Random.Range(1, dropChance)) == 1)
		{
			GameObject drop = Instantiate(powerUps[Random.Range(1, powerUps.Count)], transform.position, Quaternion.identity) as GameObject;
			drop.GetComponent<PowerUpObject>().value = Random.Range(minDuration, maxDuration);
		}else if(Mathf.Round(Random.Range(1, healthChance)) == 1)
		{
			GameObject drop = Instantiate(powerUps[0], transform.position, Quaternion.identity) as GameObject;
			drop.GetComponent<PowerUpObject>().value = Random.Range(20, maxHealthDrop);
		}
		Destroy(gameObject);
	}

	void Fire()
	{
		foreach(Transform g in bulletSpawns)
		{
			GameObject bullet = _bulletPool.GetPooledObject();
			if(bullet == null)
				return;
			bullet.transform.position = g.position;
			bullet.transform.rotation = g.rotation;
			bullet.SetActive(true);
			bullet.rigidbody.AddForce(g.forward * bulletSpeed);
			bullet.GetComponent<BulletController>().SetStats(_unitStats.damageOutput);
		}
	}
	public void Pause()
	{
		_pauseTime = Time.time;
	}
	
	public void UnPause()
	{
		_nextFireTime += (Time.time- _pauseTime);
	}
}
