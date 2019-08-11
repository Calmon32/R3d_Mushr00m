using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_controller : MonoBehaviour {

    private static int guiSwitches;
    private int healthPoints;
    private Transform player;
    public bool destroyHack;
    public Text hackText;

    void Start() {
        player = GameObject.FindWithTag("player").transform;
        destroyHack = false;

    }

    // Use this for initialization
    void Update () {
        guiSwitches = player_controller.nrSwitches;
        
        player_controller pc = player.GetComponent<player_controller>();
        healthPoints = pc.health;
    }

    // Update is called once per frame
    void OnGUI () {

        //GUI.Box(new Rect(80, 20, 130, 20), "Switches offline: " + guiSwitches + "/3");
        //GUI.Box(new Rect(20, 20, 50, 20), "HP: " + healthPoints);
        //GUI.Label(new Rect(20,20,200,20), "Switches offline: " + guiSwitches + "/3");
        if (destroyHack)
        {
            hackText.text = "Press 'R' to destroy.\nHold 'E' to hack.";
        }
        else
        {
            hackText.text = "";
        }

        if (guiSwitches == 3)
        {
            /*GameObject door = GameObject.Find("mainSwitch");
            switchMain_controller sMc = door.GetComponent<switchMain_controller>();
            GUI.Box(new Rect(20, 60, 130, 20), "" + sMc.health);*/
        }
	}
}
