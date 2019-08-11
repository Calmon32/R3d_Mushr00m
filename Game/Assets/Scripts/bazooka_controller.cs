using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bazooka_controller : MonoBehaviour {

    public float bzkRadius;
    private Rigidbody2D rb;
    public float bzkspeed = 500;
    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody2D>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = mousePosition - new Vector2(transform.position.x, transform.position.y);
        mouseDir = mouseDir.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseDir);
        rb.AddForce(mouseDir * bzkspeed);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "player")
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bzkRadius);
            foreach (Collider2D hit in colliders)
            {
                if (hit.gameObject.tag == "enemy")
                {
                    Destroy(hit.gameObject);

                }
            }
            Destroy(gameObject);
        }
    }
}
