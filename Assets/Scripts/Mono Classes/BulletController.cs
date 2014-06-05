using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float damage;
	public float lifeTime = 3;
	public GameObject hitEffect;

	private Vector3 _prePauseVel;
	private Vector3 _prePauseAngVel;
	private float _destroyTime;
	private float _pauseTime;
	private bool _isEnemy = false;

	void Start()
	{
		if(gameObject.tag == "enemyShot")
			_isEnemy = true;
	}

	void OnEnable()
	{
		_destroyTime = Time.time + lifeTime;
	}

	void Update()
	{
		if(Time.time >= _destroyTime)
		{
			gameObject.SetActive(false);
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
	}

	public void SetStats(float dmg)
	{
		damage = dmg;
	}

	void OnTriggerEnter(Collider c)//Collision
	{
		if(_isEnemy)
		{
			if(c.tag == "Player")
			{
				c.GetComponent<PlayerController>().DealDamage(damage);
				Hit();
			}
		}else
		{
			if(c.tag == "enemy")
			{
				c.GetComponent<EnemyController>().DealDamage(damage);
				Hit();
			}
		}
	}

	void Hit()
	{
		Instantiate(hitEffect, transform.position, Quaternion.identity);
		_destroyTime = Time.time;
	}

	public void Pause()
	{
		_pauseTime = Time.time;
		_prePauseVel = rigidbody.velocity;
		rigidbody.velocity = Vector3.zero;
		_prePauseAngVel = rigidbody.angularVelocity;
		rigidbody.angularVelocity = Vector3.zero;
		particleSystem.Pause();
	}
	
	public void UnPause()
	{
		_destroyTime += (Time.time- _pauseTime);
		rigidbody.velocity = _prePauseVel;
		rigidbody.angularVelocity = _prePauseAngVel;
		particleSystem.Play();
	}
}
