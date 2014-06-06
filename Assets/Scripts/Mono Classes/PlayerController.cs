using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public Vector2 lowerBound;
	public Vector2 upperBound;
	public float MoveSpeed = 10;
	public float MouseSpeed = 20;
	public float fireRate = 1;
	public GameObject basicBullet;
	public List<Transform> bulletSpawns;
	public float bulletSpeed = 1000;
	public float damage = 10;
	public float maxHealth = 100;
	public Texture2D healthBar;
	public GUISkin skin;
	public GameObject explosion;
	public float paralaxAmmount = 2;
	public bool canShoot = true;

	private Vector3 _curPosition;
	private ControlMap _controls;
	private DataMap _data;
	private float _nextFireTime;
	private float _pauseTime;
	private float _health;
	private float _paralax;
	private Transform _cam;
	private ObjectPooler _basicBulletPool;
	private float _powerEndTime;
	private PowerUp _curPower;
	private BeamGunner _bGunner;
	private bool _isPaused = false;
	private bool _mouseMode;

	// Use this for initialization
	void Start () 
	{
		_curPower = PowerUp.None;
		_bGunner = GetComponent<BeamGunner>();
		CreateBulletPool();
		_controls = GameObject.Find("_GameRegistry").GetComponent<ControlMap>();
		_data = GameObject.Find("_GameRegistry").GetComponent<DataMap>();
		_nextFireTime = Time.time + fireRate;
		_health = maxHealth;
		_cam = Camera.main.transform;
		_mouseMode = _data.GetBool("MouseMode");

	}

	void CreateBulletPool()
	{
		_basicBulletPool = new ObjectPooler();
		_basicBulletPool.CreateObjectPool(basicBullet, 18, true);
	}
	// Update is called once per frame
	void Update () 
	{
		if(_isPaused == false)
		{
			if(_mouseMode)
			{
				RaycastHit hit;
				Vector3 rawPos = Input.mousePosition;
				Vector3 mPos = new Vector3(rawPos.x, Screen.height - rawPos.y, 0);
				Physics.Raycast(Camera.main.ScreenPointToRay(rawPos), out hit);
				if(hit.point != Vector3.zero)
				{
					Vector3 hp = new Vector3(hit.point.x, hit.point.y, 0);
					_curPosition = Vector3.Lerp(_curPosition, hp, MouseSpeed * Time.deltaTime);
				}
			}else
			{
				if(Input.GetKey(_controls.GetKey("Percise Move")))
				{
					if(Input.GetKey(_controls.GetKey("Up")))
					{
						_curPosition.y += MoveSpeed * Time.deltaTime;
					}
					if(Input.GetKey(_controls.GetKey("Down")))
					{
						_curPosition.y -= MoveSpeed * Time.deltaTime;
					}
					if(Input.GetKey(_controls.GetKey("Left")))
					{
						_curPosition.x -= MoveSpeed * Time.deltaTime;
					}
					if(Input.GetKey(_controls.GetKey("Right")))
					{
						_curPosition.x += MoveSpeed * Time.deltaTime;
					}
				}else
				{
					if(Input.GetKey(_controls.GetKey("Up")))
					{
						_curPosition.y += MoveSpeed * Time.deltaTime * 2;
					}
					if(Input.GetKey(_controls.GetKey("Down")))
					{
						_curPosition.y -= MoveSpeed * Time.deltaTime * 2;
					}
					if(Input.GetKey(_controls.GetKey("Left")))
					{
						_curPosition.x -= MoveSpeed * Time.deltaTime * 2;
					}
					if(Input.GetKey(_controls.GetKey("Right")))
					{
						_curPosition.x += MoveSpeed * Time.deltaTime * 2;
					}
				}
			}
			ClampPos();
			transform.position = _curPosition;
			//Fire Bullets
			if(canShoot)
			{
				if(Time.time >= _nextFireTime)
				{
					Fire();
					_nextFireTime = Time.time + fireRate;
				}
			}
			if(_curPower != PowerUp.None)
			{
				if(_powerEndTime <= Time.time)
				{
					_curPower = PowerUp.None;
					canShoot = true;
					_bGunner.isFiring = false;
				}
			}
			if(_curPosition.y < 0)
			{
				_paralax = paralaxAmmount * (_curPosition.y/upperBound.y);
			}
			if(_curPosition.y > 0)
			{
				_paralax = paralaxAmmount * (_curPosition.y/lowerBound.y) * -1;
			}
			_cam.position = new Vector3(_cam.position.x, _paralax, _cam.position.z);
		}
	}

	void OnGUI()
	{
		GUI.skin = skin;
		GUI.DrawTexture(new Rect(0,0, Screen.width*(_health/maxHealth), 16), healthBar);
	}

	//Bullet Fire
	void Fire()
	{
		if(_basicBulletPool == null)
			CreateBulletPool();
		foreach(Transform g in bulletSpawns)
		{
			GameObject bullet = _basicBulletPool.GetPooledObject();
			if(bullet == null)
				return;
			bullet.transform.position = g.position;
			bullet.transform.rotation = g.rotation;
			bullet.SetActive(true);
			bullet.rigidbody.AddForce(g.forward * bulletSpeed);
			bullet.GetComponent<BulletController>().SetStats(damage);
		}
	}

	public void ApplyPower(PowerUp type, float value)
	{
		if(type == PowerUp.Health)
		{
			_health += value;
			if(_health > maxHealth)
				_health = maxHealth;
		}else if(type == PowerUp.Laser)
		{
			_powerEndTime = Time.time + value;
			_curPower = type;
			canShoot = false;
			_bGunner.isFiring = true;
			_bGunner.damage = damage * 100;
		}
	}

	void Die()
	{
		Instantiate(explosion, transform.position, Quaternion.identity);
		Camera.main.GetComponent<WaveMaster>().PlayerDeath();
		Destroy(gameObject);
	}

	public void DealDamage(float d)
	{
		_health -= d;
		if(_health <= 0)
			Die();
	}

	public void Pause()
	{
		_isPaused = true;
		_pauseTime = Time.time;
	}

	public void UnPause()
	{
		_isPaused = false;
		_nextFireTime += (Time.time- _pauseTime);
	}

	void ClampPos()
	{
		_curPosition.x = Mathf.Clamp(_curPosition.x, lowerBound.x, upperBound.x);
		_curPosition.y = Mathf.Clamp(_curPosition.y, lowerBound.y, upperBound.y);
	}
}
