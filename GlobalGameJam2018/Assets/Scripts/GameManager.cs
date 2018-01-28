using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float maxTime = 120;
	public float curTime = 120;

	public int peoplesLeft;
	public List<GameObject> zombies;
	public List<GameObject> peobles;
	private GameObject[] _peobles;
	
	//UI
	public Text textInfected;
	public Text textPeople;
	public Text textTimeLeft;


	// Use this for initialization
	void Start ()
	{

		textInfected.text = "PeopleInfected: " + zombies.Count;
		textPeople.text = "PeopleLeft: " + peoplesLeft;
		
		_peobles = GameObject.FindGameObjectsWithTag("Persons");
		
		for (int i = 0; i < _peobles.Length; i++)
		{
			peobles.Add(_peobles[i]);
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		curTime -= Time.deltaTime;
		textTimeLeft.text = "SEC:" + (int)curTime;
		textInfected.text = "PeopleInfected: " + zombies.Count;
		textPeople.text = "PeopleLeft: " + peoplesLeft;
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
			SceneManager.LoadScene("GUI");
		}
	}
}
