using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {
    //Public Variable
    public GameObject main; //to get main panel visible
    public GameObject credits; //to get credits panel visible
    public GameObject howtoPlay; //to get how to play panel visible

    //Private variables
    private bool isVisible = false; //to manage the visualization of Game's Instrution

    public void StartGame() {
        //Application.LoadLevel("Mecanicas");
        SceneManager.LoadScene("Mecanicas");
    }

    public void HowToPlay() {
        main.SetActive(false);
        howtoPlay.SetActive(true);
    }

    public void ReturninCredits()
    {
        main.SetActive(true);
        credits.SetActive(false);
    }

    public void ReturninHowToPlay()
    {
        main.SetActive(true);
        howtoPlay.SetActive(false);
    }

    public void Credits(){
        main.SetActive(false);
        credits.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }
	
}
