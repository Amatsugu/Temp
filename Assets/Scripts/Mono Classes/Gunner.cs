using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gunner : MonoBehaviour {

	public enum GunType {
		Spread,
		Spiral
	}

	public GameObject gunModel;
	public GunType gunType = GunType.Spiral;
	public float angleRange = 45;
	public float angleIncrement = 45;
	public ObjectPoolerWorld projectiles;
	public float fireRate = 1;
	
	private float _nextFireTime = 0;
	private float _curShotAngle = 0;
	private EnemyController _eController;

	void Start()
	{
		projectiles = GameObject.Find("_EnemyBulletPool").GetComponent<ObjectPoolerWorld>();
		_nextFireTime = Time.time + fireRate;
		_eController = GetComponent<EnemyController>();
		//gunType = Random.Range(0,2);
	}

	void Update()
	{
		if(angleIncrement < 1)
		{
			angleIncrement = 1;
		}
		if(angleRange < 0)
			angleRange = 0;
		if(gunType == GunType.Spiral)
		{
			Vector3 gmPos = gunModel.transform.position;
			Vector3 spawnPos = new Vector3(gmPos.x +(Mathf.Sin(_curShotAngle*Mathf.Deg2Rad)*2), gmPos.y + (Mathf.Cos(_curShotAngle*Mathf.Deg2Rad)*2), gmPos.z);
			Vector3 spawnPos2 = new Vector3(gmPos.x +(Mathf.Sin(_curShotAngle*Mathf.Deg2Rad)*-2), gmPos.y + (Mathf.Cos(_curShotAngle*Mathf.Deg2Rad)*-2), gmPos.z);

			Debug.DrawLine(gmPos, spawnPos, Color.cyan);
			Debug.DrawLine(gmPos, spawnPos2, Color.blue);
			if(Time.time >= _nextFireTime)
			{
				Fire(spawnPos, Quaternion.Euler(new Vector3((_curShotAngle)-90, 90,0)));
				Fire(spawnPos2, Quaternion.Euler(new Vector3((_curShotAngle*-1)+90, -90,0)));
				_nextFireTime = Time.time + fireRate;
				_curShotAngle += angleIncrement;
				if(_curShotAngle > 360)
				{
					_curShotAngle = _curShotAngle-360;
				}
			}
			
		}
		if(gunType == GunType.Spread)
		{
			if(Time.time >= _nextFireTime)
			{
				for(_curShotAngle = (-90-angleRange); _curShotAngle <= (-90+angleRange); _curShotAngle += angleIncrement)
				{
					Vector3 gmPos = gunModel.transform.position;
					Vector3 spawnPos = new Vector3(gmPos.x +(Mathf.Sin(_curShotAngle*Mathf.Deg2Rad)*2), gmPos.y + (Mathf.Cos(_curShotAngle*Mathf.Deg2Rad)*2), gmPos.z);
					Fire(spawnPos, Quaternion.Euler(new Vector3((_curShotAngle)-90, 90,0)));
					Debug.DrawLine(gmPos, spawnPos, Color.blue);
				}
				_nextFireTime = Time.time + fireRate;
			}
		}
	}
	void Fire(Vector3 g, Quaternion r)
	{
		if(projectiles == null)
			return;
		GameObject bullet = projectiles.GetPooledObject();
		if(bullet == null)
			return;
		bullet.transform.position = g;
		bullet.transform.rotation = r;
		bullet.SetActive(true);
		bullet.rigidbody.AddForce(bullet.transform.forward * _eController.bulletSpeed);
		bullet.GetComponent<BulletController>().SetStats(GetComponent<UnitStats>().damageOutput);
	}
}
