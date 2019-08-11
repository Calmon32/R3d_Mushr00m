using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour {

    private static int switchesOff;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        switchesOff = player_controller.nrSwitches;
        if (switchesOff == 3)
        {
            Destroy(gameObject);
        }
    }
}
