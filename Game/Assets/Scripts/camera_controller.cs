using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour {

    public Transform player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("player").transform;


    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.transform.position.x,player.transform.position.y,-20);
	}
}
