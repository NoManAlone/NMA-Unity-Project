using UnityEngine;
using System.Collections;

public class BasicPatrol : MonoBehaviour
{
    public bool movingRight, patrolling, grounded;
    public float moveSpeed = 8;
    public float changeDirLength = 2;

    Rigidbody2D enemyRB;
    BoxCollider2D enemyCollider;

    public LayerMask collisions;

    RaycastHit2D hit;

    // Use this for initialization
    void Start ()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();

        StartCoroutine(Patrol());
    }
	
	// Update is called once per frame
	void Update ()
    {

        //Grouded check
        if (Physics2D.Raycast(enemyCollider.bounds.center, -Vector2.up, 1.8f, collisions))
            grounded = true;

        else
        {
            grounded = false;
        }

        if (GameManager.debug)
            Debug.DrawLine(enemyCollider.bounds.center, new Vector2(enemyCollider.bounds.center.x, enemyCollider.bounds.center.y - 1.8f), Color.yellow);

        //Wall check
        if (patrolling)// if at wall change direction
        {
            if (movingRight)
            {
                hit = Physics2D.Raycast(enemyCollider.bounds.center, Vector2.right, changeDirLength, collisions);

                if (GameManager.debug)
                    Debug.DrawRay(enemyCollider.bounds.center, Vector2.right * 3, Color.red);

            }

            else
            {
                hit = Physics2D.Raycast(enemyCollider.bounds.center, -Vector2.right, changeDirLength, collisions);

                if (GameManager.debug)
                    Debug.DrawRay(enemyCollider.bounds.center, -Vector2.right * 3, Color.red);
            }

            if (hit)
                ChangeDirection();
        }

        //Edge Check
        if (patrolling && grounded)//if close at edge change direction 
        {
            if (movingRight)
            {
                hit = Physics2D.Raycast(enemyCollider.bounds.center, new Vector2(1, -.5f), 5, collisions);

                if (GameManager.debug)
                    Debug.DrawRay(enemyCollider.bounds.center, new Vector2(1, -.5f) * 4, Color.blue);
            }

            else
            {
                hit = Physics2D.Raycast(enemyCollider.bounds.center, new Vector2(-1, -.5f), 5, collisions);

                if(GameManager.debug)
                    Debug.DrawRay(enemyCollider.bounds.center, new Vector2(-1, -.5f) * 4, Color.blue);
            }

            if (!hit)
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Fan")
        {
            if(collider.GetComponent<Power>().powered == true)
            {
                enemyRB.velocity = Vector2.zero;
            }
        }
    }
}
