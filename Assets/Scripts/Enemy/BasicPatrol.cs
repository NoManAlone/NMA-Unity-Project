using UnityEngine;
using System.Collections;

public class BasicPatrol : MonoBehaviour
{
    public bool movingRight, patrolling, grounded;
    public float moveSpeed = 8;
    public float changeDirLength = 2;

    Rigidbody2D enemyRB;
    BoxCollider2D collider;

    public LayerMask collisions;

    RaycastHit2D hit;

    // Use this for initialization
    void Start ()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        StartCoroutine(Patrol());
    }
	
	// Update is called once per frame
	void Update ()
    {

        //Grouded check
        Debug.DrawLine(collider.bounds.center, new Vector2(collider.bounds.center.x, collider.bounds.center.y - 1.7f), Color.magenta);
        if (Physics2D.Raycast(collider.bounds.center, -Vector2.up, 1.7f, collisions))
            grounded = true;

        else
        {
            grounded = false;
        }  

        //Wall check
        if (patrolling)
        {
            if(movingRight)
            {
                hit = Physics2D.Raycast(collider.bounds.center, Vector2.right, changeDirLength, collisions);
                Debug.DrawLine(collider.bounds.center, new Vector2(collider.bounds.center.x + changeDirLength, collider.bounds.center.y), Color.magenta);  
            }

            else
            {
                hit = Physics2D.Raycast(collider.bounds.center, -Vector2.right, changeDirLength, collisions);
                Debug.DrawLine(collider.bounds.center, new Vector2(collider.bounds.center.x - changeDirLength, collider.bounds.center.y), Color.magenta);
            }

            if (hit)
                ChangeDirection();
        }
    }

    IEnumerator Patrol()
    {
        patrolling = true;
               
        while(patrolling)
        {
            while (grounded && movingRight)
            {
                enemyRB.velocity = new Vector2(moveSpeed, enemyRB.velocity.y);
                yield return null;
            }

            while (grounded && !movingRight)
            {
                enemyRB.velocity = new Vector2(-moveSpeed, enemyRB.velocity.y);
                yield return null;
            }
       
            yield return null;
        } 
    }

    void ChangeDirection()
    {
        if (movingRight)
            movingRight = false;

        else
            movingRight = true;
    }
}
