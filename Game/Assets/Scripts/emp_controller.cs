using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emp_controller : MonoBehaviour {

    private Rigidbody2D rb;
    public float speed = 500;
    public float expRadius;
	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody2D>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = mousePosition - new Vector2(transform.position.x, transform.position.y);
        mouseDir = mouseDir.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseDir);
        rb.AddForce(mouseDir * speed);
        StartCoroutine(cancelThrow(0.5f));
        StartCoroutine(timerBlow(3));
        //Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        
	}

    IEnumerator cancelThrow(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
    }

    IEnumerator timerBlow(float time)
    {
        yield return new WaitForSeconds(time);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, expRadius);
        foreach (Collider2D hit in colliders)
        {
            if (hit.gameObject.tag == "enemy")
            {
                hit.gameObject.GetComponent<enemy_controller>().enabled = false;
                
            }
        }
        Destroy(gameObject);
    }
}
