using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public float maxTime = 120;

	public int peoplesLeft;
	public List<GameObject> zombies;
	public List<GameObject> peobles;
	private GameObject[] _peobles;


	// Use this for initialization
	void Start () {
		
		_peobles = GameObject.FindGameObjectsWithTag("Persons");
		
		for (int i = 0; i < _peobles.Length; i++)
		{
			peobles.Add(_peobles[i]);
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		peoplesLeft = peobles.Count;
		
		for (int i = 0; i < peobles.Count; i++)
		{
			if (peobles[i].GetComponent<PlayerController>().isZombie)
			{
				peobles.Remove(peobles[i]);
			}
		}

		if (peoplesLeft <= 0)
		{
			//Win
		}
	}
}
