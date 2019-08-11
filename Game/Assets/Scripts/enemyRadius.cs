using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyRadius : MonoBehaviour {

    public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, 0);

    }
}
