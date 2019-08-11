using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_controller : MonoBehaviour {

    public int health = 5;
    public static int nrSwitches = 0;
    private float holdTimer = 0;
    public Slider healthSlider;
    public Text switchText;

    // Use this for initialization
    void Start () {
        nrSwitches = 0;
	}

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;
        switchText.text = "Switches offline: " + nrSwitches + "/3";
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<Rigidbody2D>().angularVelocity = 0;

        //hack&destroy
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        foreach (Collider2D hit in colliders)
        {
            GUI_controller script = GameObject.FindWithTag("MainCamera").GetComponent<GUI_controller>();
            if (hit.gameObject.tag == "enemy" && hit.gameObject.GetComponent<enemy_controller>().enabled == false)
            {              
                script.destroyHack = true;               
                if (Input.GetKey(KeyCode.E))
                { 
                    holdTimer += Time.deltaTime;
                    if (holdTimer > 2)
                    {
                        hit.gameObject.GetComponent<enemy_controller>().hacked = true;
                        hit.gameObject.GetComponent<enemy_controller>().tag = "Untagged";
                        hit.gameObject.GetComponent<enemy_controller>().enabled = true;
                        holdTimer = 0;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    Destroy(hit.gameObject);
                }
            }
            else
            {
                script.destroyHack = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "switch")
        {
            nrSwitches++;
        }

        if (col.gameObject.tag == "ammo")
        {
            Destroy(col.gameObject);
            GetComponent<player_shooting>().ammo++;
        }
    }

    public void backGear() {
        SceneManager.LoadScene("GearSelect");
    }

}