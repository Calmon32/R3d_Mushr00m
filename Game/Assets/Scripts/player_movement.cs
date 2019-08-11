using System.Collections;
using System.Collections.Generic;
using CnControls;
using UnityEngine;

public class player_movement : MonoBehaviour {

    public float speed;
    private int platform;
    public static Vector3 playerDirection;
	// Use this for initialization
	void Start () {
        if (Application.platform != RuntimePlatform.Android){
            platform = 1;
            Debug.Log("ITS 1");
            GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag ("mobileUI");
 
            foreach(GameObject go in gameObjectArray)
            {
                go.SetActive (false);
                Debug.Log("remove");
            }
        } else {
            platform = 2; 
            Debug.Log("ITS 2");
        }
        playerDirection = Vector3.up;
	}

	// Update is called once per frame
	void FixedUpdate () {

        if (platform == 1){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
            playerDirection = mousePos - transform.position;
        } else if (platform == 2) {
            Vector3 vec = new Vector3(CnInputManager.GetAxis("HorizontalView"), CnInputManager.GetAxis("VerticalView"), 0f);
            if(vec != Vector3.zero) {
                playerDirection = vec;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, vec);
            }
        }

        /*
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * speed, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * speed, Space.World);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * speed, Space.World);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * speed, Space.World);
        }
        */

        var vector = new Vector3(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"), 0f);
        Debug.Log(vector);
        if (vector.x > 0.4f) {
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
        } else if(vector.x < -0.4f) {
            transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
        }

        if (vector.y > 0.4f) {
            transform.Translate(Vector2.up * speed * Time.deltaTime, Space.World);
        } else if(vector.y < -0.4f) {
            transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);
        }

        //transform.Translate(movementVectorX * speed , Space.World);
        //transform.Translate(movementVectorY * speed , Space.World);
    }
}
