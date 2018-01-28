using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
	//datos
	public float speed = 10f;
	public float rotationSpeed = 450f;
	
	//stats
	public int curHealth = 10;
	public int damage = 5;
	public bool isZombie = false;
	
	//AI
	public GameObject[] waypoints;

	private Rigidbody _rigidbody;
	private CapsuleCollider _capsuleCollider;
	private Quaternion _targetRotation;
	private NavMeshAgent _navMeshAgent;
	
	

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		_navMeshAgent = GetComponent<NavMeshAgent>();
		
		//AI
		if (!isZombie)
		{
			_navMeshAgent.speed = speed;
			_navMeshAgent.angularSpeed = rotationSpeed;
			_navMeshAgent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].transform.position);
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (isZombie)
		{
			AWSDController();
		}
		else
		{
			AIcontroller();
		}
	}
	//funcion para mover con las teclas AWSD.
	void AWSDController()
	{
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

	//detectar collisiones
	private void OnCollisionEnter(Collision other)
	{
		//Nuestro jugador
		if (other.collider.tag == "Persons" || other.collider.tag == "Cop" && this.tag == "Player")
		{
			other.collider.GetComponent<PlayerController>().ApplyDamage(damage);
		}
		
		//los polis
		if (other.collider.tag == "Player" && this.tag == "Cop" && !this.isZombie)
		{
			other.collider.GetComponent<PlayerController>().ApplyDamage(damage);
		}
		
	}
	
	//Aplica el daño a nuestro personaje
	public void ApplyDamage(int damage)
	{
		if (curHealth - damage > 0)
		{
			curHealth -= damage;
			//StartCoroutine(GetHit());
		}
		else
		{
			curHealth = 0;
			Death();
		}
	}

	public void Death()
	{
		if (this.tag == "Persons" || this.tag == "Cop" && !this.isZombie)
		{
			_navMeshAgent.Stop();
			TransformZombie(this.transform, Color.green);
			this.isZombie = true;
		}
		else
		{
			Debug.Log("Player is DEATH!!");
		}
	}
	
	//funcion recursiva.
	private void TransformZombie(Transform victima, Color ZombieColor)
	{
		foreach (Transform child in victima)
		{
			TransformZombie(child, ZombieColor);
			child.GetComponent<Renderer>().material.color = ZombieColor;
		}
	}

	void AIcontroller()
	{
		Debug.Log(_navMeshAgent.remainingDistance);
		if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= 0.5f)
		{
			_navMeshAgent.destination = waypoints[Random.Range(0, waypoints.Length)].transform.position;
		}
	}
}
