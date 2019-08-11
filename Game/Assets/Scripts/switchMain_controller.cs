using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchMain_controller : MonoBehaviour {

    public int health;
	// Use this for initialization
	void Start () {

        health = 5;
	}
	
	// Update is called once per frame
	void Update () {

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
