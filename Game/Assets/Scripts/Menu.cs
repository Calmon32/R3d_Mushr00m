using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startGame() {
        SceneManager.LoadScene("GearSelect");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
