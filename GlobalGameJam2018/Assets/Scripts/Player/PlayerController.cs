using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{

	public float speed = 10f;
	public float rotationSpeed = 450f;

	private Rigidbody _rigidbody;
	private CapsuleCollider _capsuleCollider;
	
	private Quaternion _targetRotation;

	// Use this for initialization
	void Start ()
	{

		_rigidbody = GetComponent<Rigidbody>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
		
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

	}
	
	// Update is called once per frame
	void Update () {
		//agarramos los inputs del usuario
		float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
		float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
		Vector3 input = new Vector3(h, 0, v);

		if (input != Vector3.zero)
		{
			_targetRotation = Quaternion.LookRotation(input);
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, _targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
		}

		Vector3 move = input;
		move *= speed * 10;
		move *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? 0.7f : 1f;
		
		_rigidbody.AddForce(move * Time.deltaTime, ForceMode.Impulse);
		
	}
}
