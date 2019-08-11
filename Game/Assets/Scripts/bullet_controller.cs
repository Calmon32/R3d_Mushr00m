using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_controller : MonoBehaviour
{

    public bool enemy;
    public bool hackedenemy;
    private Rigidbody2D rb;
    public float speed = 500;
    public int dmg;
    Vector2 mousePosition;
    Vector2 playerPosition;
    Vector2 enemypos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemy) {

            Transform player = GameObject.FindWithTag("player").transform;
            playerPosition = player.transform.position;
            Vector2 mouseDir = playerPosition - new Vector2(transform.position.x, transform.position.y);
            mouseDir = mouseDir.normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseDir);
            rb.AddForce(mouseDir * speed);
        }

        else if (hackedenemy)
        {

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            foreach (Collider2D hit in colliders)
            {
                //Debug.Log("hits feitos");
                if (hit.gameObject.tag == "enemy")
                {
                    Transform enemy = hit.gameObject.transform;
                    enemypos = enemy.transform.position;
                    Vector2 mouseDir = enemypos - new Vector2(transform.position.x, transform.position.y);
                    mouseDir = mouseDir.normalized;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseDir);
                    rb.AddForce(mouseDir * speed);
                }
            }
        }

        else
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDir = player_movement.playerDirection;
            mouseDir = mouseDir.normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseDir);
            rb.AddForce(mouseDir * speed);
        }
        Destroy(gameObject,5f);
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (enemy)
        {
            if (col.gameObject.tag == "player")
            {
                player_controller pc = col.GetComponent<player_controller>();
                pc.health = pc.health - dmg;
                Destroy(gameObject);
            }

            else if (col.gameObject.tag != "enemy" & col.gameObject.tag != "bullet") {
                Destroy(gameObject);
            }
        }

        else if (hackedenemy)
        {
            if (col.gameObject.tag == "enemy")
            {
                enemy_controller ec = col.GetComponent<enemy_controller>();
                ec.health = ec.health - dmg;
                Destroy(gameObject);
            }

            if (col.gameObject.tag == "player")
            {
                player_controller pc = col.GetComponent<player_controller>();
                pc.health = pc.health - dmg;
                Destroy(gameObject);
            }

            if (col.gameObject.tag == "wall")
            {
                Destroy(gameObject);
            }
        }

        else {
            if (col.gameObject.tag != "player")
            {
                if (col.gameObject.tag == "enemy")
                {
                    enemy_controller ec = col.GetComponent<enemy_controller>();
                    ec.health = ec.health - dmg;
                }
                Destroy(gameObject);

                if (col.gameObject.tag == "mainSwitch")
                {
                    switchMain_controller sMc = col.GetComponent<switchMain_controller>();
                    sMc.health = sMc.health - dmg;
                }
            }
        }
    }
}
