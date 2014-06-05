using UnityEngine;
using System.Collections;

public class BeamGunner : MonoBehaviour 
{
	public LineRenderer line;
	public Transform hitEffect;
	public Transform startEffect;
	public float damage = 1000;
	public float speed = 5;
	public float maxLength = 50;
	public bool isFiring = true;

	private float _curLenght = 0;
	private float _curDesLen = 0;
	private Transform _thisTransform;

	void Start () 
	{
		_thisTransform = transform;
		_curDesLen = maxLength;
	}

	public void Pause()
	{
		hitEffect.particleSystem.Pause();
		startEffect.particleSystem.Pause();
		particleSystem.Pause();
	}

	public void UnPause()
	{
		hitEffect.particleSystem.Play();
		startEffect.particleSystem.Play();
		particleSystem.Play();
	}

	void Update () 
	{
		if(isFiring)
		{
			if(!line.enabled)
			{
				line.enabled = true;
				line.particleSystem.Play();
				hitEffect.light.enabled = true;
				startEffect.light.enabled = true;
			}
			line.SetPosition(0, _thisTransform.position);
			RaycastHit hit;
			Physics.Raycast(_thisTransform.position, _thisTransform.forward, out hit, maxLength);
			if(hit.point == Vector3.zero)
			{
				_curDesLen = maxLength;
				hitEffect.position = _thisTransform.position;
				hitEffect.particleSystem.Clear();
			}else
			{
				_curDesLen = Vector3.Distance(_thisTransform.position, hit.point);
				hitEffect.position = new Vector3(_thisTransform.position.x + _curLenght, _thisTransform.position.y, 0);
				if(hit.collider.tag == "enemy")
				{
					hit.collider.GetComponent<EnemyController>().DealDamage(damage*Time.deltaTime);
				}
			}
			line.SetPosition(1, new Vector3(_thisTransform.position.x + _curLenght, _thisTransform.position.y, 0));
			if(_curLenght < _curDesLen)
				_curLenght += speed * Time.deltaTime;
			else if(_curLenght > _curDesLen)
			{
				_curLenght = _curDesLen;
			}
		}else
		{
			if(line.enabled)
			{
				line.enabled = false;
				line.particleSystem.Stop();
				hitEffect.light.enabled = false;
				startEffect.light.enabled = false;
				_curLenght = 0;
			}
		}
	}
}
