using UnityEngine;
using System.Collections;

public class PropController : MonoBehaviour {
	public enum motionMode{
		physics,
		transform
	}

	public float speed = 5;
	public float lifeTime = 10;
	public motionMode mode;

	private float _pauseTime;
	private Vector3 _prePauseVel;
	private Vector3 _prePauseAngVel;
	private float _destroyTime;
	private bool _isPaused = false;

	void Start () 
	{
		_destroyTime = Time.time + lifeTime;
		if(mode == motionMode.physics)
			rigidbody.AddForce(speed * Vector3.right * -1);
		else
			transform.rotation = Quaternion.identity;
	}
	
	void Update () 
	{
		if(!_isPaused)
		{
			if(Time.time >= _destroyTime)
			{
				Destroy(gameObject);
			}
			if(mode == motionMode.transform)
				transform.Translate(Vector3.right * speed * -1 * Time.deltaTime);
		}
	}

	public void Pause()
	{
		_isPaused = true;
		_pauseTime = Time.time;
		if(particleSystem != null)
			particleSystem.Pause();
		if(mode == motionMode.physics)
		{
			_prePauseVel = rigidbody.velocity;
			rigidbody.velocity = Vector3.zero;
			_prePauseAngVel = rigidbody.angularVelocity;
			rigidbody.angularVelocity = Vector3.zero;
		}
	}
	
	public void UnPause()
	{
		_isPaused = false;
		_destroyTime += (Time.time- _pauseTime);
		if(particleSystem != null)
			particleSystem.Play();
		if(mode == motionMode.physics)
		{
			rigidbody.velocity = _prePauseVel;
			rigidbody.angularVelocity = _prePauseAngVel;
		}
	}
}
