using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controller : MonoBehaviour {

    public int health;
    public float calmSpeed = 0.05f;
    public float runSpeed = 0.2f;
    public int group;


    public Transform ammo;

    public Transform bullet;
    public Transform bulletHacked;
    private Transform player;
    bool calm = true;

    Rigidbody2D rb;
    private Vector2 _startPosition;

    private Transform facingPoint;

    private GameObject[] patrolPoints;
    private Vector3 patrolDest = Vector3.zero;
    private int currentPoint;

    float timer;
    float waitingTime = 0.5f;

    float tim;

    public Transform pathingTarget;
    private List<Vector2> path;


    public bool hacked = false;
    public bool disabled;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("player").transform;
        _startPosition = transform.position;
        patrolPoints = GameObject.FindGameObjectsWithTag("PPoints"+group);
        Debug.Log(patrolPoints.Length);
        facingPoint = transform.Find("facing").transform;
    }
	
	
	void FixedUpdate () {

        if (hacked)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            foreach (Collider2D hit in colliders)
            {
                //Debug.Log("hits feitos");
                if (hit.gameObject.tag == "enemy")
                {
                    timer += Time.deltaTime;
                    if (timer > waitingTime) {

                        GameObject dynamicParent = GameObject.Find("DynamicObjects");
                        Transform newObject = Instantiate(bulletHacked, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                        newObject.parent = dynamicParent.transform;
                        Debug.Log("success em principio");
                        timer = 0;
                    }
                }
            }
        }
        else
        {
            if (health <= 0)
            {
                Instantiate(ammo, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity);
                Destroy(gameObject);
            }

            if (calm)
            {
                if (patrolDest.Equals(Vector3.zero))
                {
                    GameObject patrolPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
                    patrolDest = patrolPoint.transform.position;
                    path = NavMesh2D.GetSmoothedPath(transform.position, patrolDest);
                    Debug.Log("RESET");
                }
                else {
                    float playerDist = Vector2.Distance(transform.position, patrolDest);
                    if (playerDist < 0.5f) {
                        patrolDest = Vector3.zero;
                    }
                    if (path != null && path.Count != 0)
                    {
                        moveToPosition(new Vector3(path[0].x, path[0].y, 0));
                        //Debug.Log(path[0].x);
                        //Debug.Log(path[0].y);
                        //Debug.Log(Vector2.Distance(transform.position, path[0]));
                        if (Vector2.Distance(transform.position, path[0]) < 0.1f)
                        {
                            path.RemoveAt(0);
                            Debug.Log("REMOVED");
                        }
                    }
                }
                
            }
            else
            {
                float playerDist = Vector2.Distance(transform.position, player.transform.position);
                if (playerDist > 5)
                {
                    path = NavMesh2D.GetSmoothedPath(transform.position, player.transform.position);

                    if (path != null && path.Count != 0)
                    {
                        if (currentPoint == 0) {
                            moveToPosition(new Vector3(path[1].x, path[1].y, 0));
                        }
                        if (Vector2.Distance(transform.position, path[currentPoint + 1]) < 0.01f)
                        {
                            currentPoint++;
                            moveToPosition(new Vector3(path[currentPoint].x, path[currentPoint].y, 0));
                        }
                    }
                } else {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0;
                    Vector3 playerDir = player.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, playerDir);
                }
            }

            bool tr = canSeePlayer();
            if (tr)
            {
                calm = false;

                timer += Time.deltaTime;
                Debug.Log(timer);
                if (timer > waitingTime)
                {

                    GameObject dynamicParent = GameObject.Find("DynamicObjects");
                    Transform newObject = Instantiate(bullet, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.identity) as Transform;
                    newObject.parent = dynamicParent.transform;
                    Debug.Log("SHOOT");
                    timer = 0;
                }
            }
        }
    }

    void moveToPosition(Vector3 target) {
        Vector3 playerDir = target - transform.position;
        playerDir = playerDir.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, playerDir);
        if (calm) {
            rb.velocity = playerDir * calmSpeed;
        } else {
            rb.velocity = playerDir * runSpeed;
        }
    }

    bool canSeePlayer() {
        
        float playerDist = Vector2.Distance(transform.position, player.transform.position);
        Vector3 rayDirection = player.transform.position - transform.position;
        RaycastHit2D hit = GetFirstRaycastHit(rayDirection);
        Vector3 enemyDir = transform.position - facingPoint.transform.position;
        //Debug.Log(rayDirection);
        //Debug.Log(enemyDir);
        float angle = Vector3.Angle(transform.position - player.transform.position, enemyDir);
        Debug.DrawLine(transform.position, player.transform.position, Color.green);
        Debug.DrawLine(transform.position, facingPoint.transform.position, Color.red);
        if (hit.collider != null) {
            if (hit.transform.tag == "player" && playerDist < 10)
            {
                if (angle < 55)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public RaycastHit2D GetFirstRaycastHit(Vector3 direction)
    {
        //Check "Queries Start in Colliders" in Edit > Project Settings > Physics2D
        RaycastHit2D[] hits = new RaycastHit2D[2];
        Physics2D.RaycastNonAlloc(transform.position, direction, hits);
        //hits[0] will always be the Collider2D you are casting from.
        for (int x = 0; x < hits.Length; x++) {
            if (hits[x].collider.tag == "player")
            {
                return hits[x];
            }
        }
        return hits[1];
    }
}
