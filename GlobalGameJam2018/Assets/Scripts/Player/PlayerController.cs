using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
	//datos
	public float speed = 5f;
	public float runSpeed = 10f;
	public float rotationSpeed = 450f;
	
	//stats
	public int curHealth = 10;
	public int maxHealth = 10;
	public int damage = 5;
	public bool isZombie = false;
	
	//AI
	public bool isCop = false;
	public GameObject[] waypoints;
	public GameObject[] runWaypoints;
	public enum AISTATE
	{
		Patrol,
		AtackPlayer,
		Run
	}
	
	//Audio
	public AudioClip hurtAudio;
	
	
	//GUI
	public Image panelHit; //panel para hacer efecto de hit
	private Color curColorPanelHit; //el color neutral de panel
	public Color colorHit; //el nuevo color del panel de hit

	public Slider healthBar;

	public AISTATE aiState = AISTATE.Patrol;
	public float attackRage = 2.5f;
	public float rateOfFire = 0.15f; //velocidad de disparo
	public float myTimerToShoot = 0f; //el tiempo guardado para poder disparar.

	private Rigidbody _rigidbody;
	private CapsuleCollider _capsuleCollider;
	private Quaternion _targetRotation;
	private NavMeshAgent _navMeshAgent;
	private GameManager _gameManager;
	private AudioSource _audioSource;
	
	

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		_audioSource = GetComponent<AudioSource>();
		
		//AI
		if (!isZombie)
		{
			_navMeshAgent.speed = speed;
			_navMeshAgent.angularSpeed = rotationSpeed;
			_navMeshAgent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].transform.position);
		}

		if (this.tag == "Player")
		{
			healthBar.maxValue = maxHealth;
			healthBar.value = curHealth;
		}

	}
	
	// Update is called once per frame
	void Update () {
		myTimerToShoot += Time.deltaTime; //acumula el timer

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

		if (this.isZombie)
		{
			//Nuestro jugador
			if (other.collider.tag == "Persons" || other.collider.tag == "Cop" && this.tag == "Player")
			{
				if (!other.collider.GetComponent<PlayerController>().isZombie)
				{
					other.collider.GetComponent<PlayerController>().ApplyDamage(damage);
				}
			}
		}
		else
		{
			//los polis
			if (other.collider.tag == "Player" && this.tag == "Cop" && !this.isZombie)
			{
				other.collider.GetComponent<PlayerController>().ApplyDamage(damage);
			}
		}
	}
	
	//Aplica el daño a nuestro personaje
	public void ApplyDamage(int damage)
	{
		if (curHealth - damage > 0)
		{
			curHealth -= damage;
			StartCoroutine(GetHit());
		}
		else
		{
			curHealth = 0;
			Death();
		}
	}
	
	//get hit
	IEnumerator GetHit()
	{
		_audioSource.clip = hurtAudio;
		_audioSource.Play();
		if (this.isZombie)
		{
			panelHit.color = colorHit;
			yield return new WaitForSeconds(0.2f); //esperamos 0.2 segundos para poner el efecto de hit
			panelHit.color = curColorPanelHit;

			if (this.tag == "Player")
			{
				healthBar.value = curHealth;
			}
		}
		else
		{
			if (!isCop)
			{
				aiState = AISTATE.Run;
				_navMeshAgent.speed = runSpeed;
				_navMeshAgent.angularSpeed = 800;
				_navMeshAgent.SetDestination(runWaypoints[Random.Range(0, runWaypoints.Length)].transform.position);
			}
		}
		
	}
	
	//is death
	public void Death()
	{
		if (this.tag == "Persons" || this.tag == "Cop" && !this.isZombie)
		{
			_navMeshAgent.Stop();
			TransformZombie(this.transform, Color.green);
			this.isZombie = true;
			_gameManager.zombies.Add(this.gameObject);
		}
		else
		{
			if (this.tag == "Player")
			{
				//LOOSE
				SceneManager.LoadScene("GUI");
			}
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

		switch (aiState)
		{
			//patrol
			case AISTATE.Patrol:
				
				if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= 0.5f)
				{
					_navMeshAgent.destination = waypoints[Random.Range(0, waypoints.Length)].transform.position;
				}
				break;
				//run
			case AISTATE.Run:
					
					if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= 0.5f)
					{
						_navMeshAgent.destination = runWaypoints[Random.Range(0, runWaypoints.Length)].transform.position;
					}
					break;
					
			case AISTATE.AtackPlayer:
					_navMeshAgent.destination = GameObject.FindGameObjectWithTag("Player").transform.position;

					if ( !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= attackRage)
					{
						if (myTimerToShoot >= rateOfFire && Time.deltaTime != 0)
						{
							//reiniciamos nuestro timer para volver a disparar
							myTimerToShoot = 0;
							GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ApplyDamage(damage);
						}
					}
					
					break;
		}
		
	}
}
