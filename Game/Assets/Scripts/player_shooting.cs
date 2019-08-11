using System.Collections;
using System.Collections.Generic;
using CnControls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player_shooting : MonoBehaviour {

    public Transform bullet;
    public Transform smgbullet;
    public Transform emp_nade;
    public Transform bzk;
    private Rigidbody2D rb;
    public int ammo;
    public int nadeammo = 2;
    public Text pAmmoText;
    public Text sAmmoText;
    public Text uAmmoText;
    private bool paused = false;

    float timer;
    public float waitingTime;

    private int platform;


    void Start () {

        if (Application.platform != RuntimePlatform.Android){
            platform = 1;
        } else {
            platform = 2;
        }


        rb = GetComponent<Rigidbody2D>();
        if (GearMenu.itemSelected == 1) {
            ammo = 6;
            waitingTime = 0.7f;
        } else if(GearMenu.itemSelected == 2) {
            ammo = 20;
            waitingTime = 0.3f;
        } else if(GearMenu.itemSelected == 3) {
            ammo = 3;
            waitingTime = 1.3f;
        } else if(GearMenu.itemSelected == 4) {
            ammo = 4;
            waitingTime = 0.7f;
        }
    }
	

	void Update () {
        GameObject dynamicParent = GameObject.Find("DynamicObjects");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("GearSelect");
        }

        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            if (platform == 1) {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (nadeammo > 0) {
                            Transform newObject = Instantiate(emp_nade, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            nadeammo--;
                            return;
                        }
                }
                
                if (Input.GetMouseButton(0))
                {
                    if (GearMenu.itemSelected == 1) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 2) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(smgbullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 3) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bzk, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 4) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    }
                }
            } else if(platform == 2) {
                if (CnInputManager.GetButton("Fire2"))
                {
                    if (nadeammo > 0) {
                            Transform newObject = Instantiate(emp_nade, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            nadeammo--;
                        }
                }

                if(CnInputManager.GetButton("Fire1")){
                    if (GearMenu.itemSelected == 1) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 2) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(smgbullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 3) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bzk, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    } else if(GearMenu.itemSelected == 4) {
                        if (ammo > 0) {
                            Transform newObject = Instantiate(bullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                            newObject.parent = dynamicParent.transform;
                            ammo--;
                            timer = 0;
                        }
                    }
                }
            }
        }
    }
}
