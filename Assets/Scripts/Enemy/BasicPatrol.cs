using UnityEngine;
using System.Collections;

public class BasicPatrol : MonoBehaviour 
{

	public bool patrolling, movingRight;
	float moveSpeed = 7;

	Collider2D collider;
	float lineOfSight = 5;

	RaycastHit2D hit;

	public LayerMask collisions;

	Rigidbody2D enemyRigidbody;


	// Use this for initialization
	void Start () 
	{
		collider = GetComponent<Collider2D>();

		enemyRigidbody = GetComponent<Rigidbody2D>();

		StartCoroutine(Patrol());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (movingRight) 
		{
			hit = Physics2D.Raycast (collider.bounds.center, Vector2.right, collider.bounds.extents.x + lineOfSight, collisions);
			Debug.DrawLine(transform.position, new Vector2(collider.bounds.center.x + lineOfSight, transform.position.y), Color.yellow);
		}
			

		else 
			hit = Physics2D.Raycast (collider.bounds.center, -Vector2.right, collider.bounds.extents.x + lineOfSight, collisions);

		if (hit) 
		{
			ChangeDirection ();
		}
	}

	IEnumerator Patrol()
	{
		patrolling = true;

		while (patrolling && movingRight) //Coroutine exits when these conditions are false
		{
			enemyRigidbody.velocity = new Vector2 (moveSpeed, enemyRigidbody.velocity.y);
			yield return null;//no time specified so will loop on next frame
		}

		while(patrolling && !movingRight)
		{
			enemyRigidbody.velocity = new Vector2 (-moveSpeed, enemyRigidbody.velocity.y);
			yield return null;//no time specified so will loop on next frame
		}
//		
//		while(patrolling && !movingRight)
//		{
//			transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x - 1, transform.position.y), Time.deltaTime * moveSpeed);
//			yield return null;
//		}
	}

	void ChangeDirection()
	{
		if (movingRight)
			movingRight = false;

		else
			movingRight = true;

		StartCoroutine(Patrol());
	}
}
