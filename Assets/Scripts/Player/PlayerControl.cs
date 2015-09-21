using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour 
{
	public int playerID;
	PhotonView powerMeterPhotonView;

	//Cameras
	public GameObject consoleCamera, playerCamera;

	//PowerManager
	PowerManager powerManager;

	//Input Keys
	public KeyCode left, right, jump, consoleButton;

	//bools
	public bool isPlayer1;
	public bool grounded, moving, movingRight, movingLeft, jumping, boosting, usingConsole, teleporting;
	public bool teleporterOverlap;
	bool facingRight = true;

	//Physics Values
	float moveSpeed = 10, jumpForce = 1000;
	public GameObject interactObject;

	public bool jumpedTwice = false;
	bool knockback;

	Rigidbody2D playerRigidbody;

	CircleCollider2D[] colliders;
	CircleCollider2D feetCollider;

	public LayerMask physicalColliders;
	
	Animator anim;

	public int collidingWall;

	float enemyHitPosX;

	void Awake()
	{
		powerMeterPhotonView = GameObject.Find("PowerMeter").GetComponent<PhotonView>();

		//Initialises PowerManager.
		powerManager = GameObject.Find("PowerMeter").GetComponent<PowerManager>();

		//Initialises Animator and Colliders.
		anim = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody2D>();
		colliders = gameObject.GetComponents<CircleCollider2D>();
		feetCollider = colliders[1];
	}

	void Update()
	{
		Controls();
		Animation();
	}

	void FixedUpdate()
	{
		Movement();
	}

	void OnTriggerEnter2D(Collider2D collidercheck)
	{  
		//JumpPad
		if(collidercheck.gameObject.tag=="JumpPad")
		{
			GameObject jumpad = collidercheck.gameObject;
			
			if(jumpad.GetComponent<JumpPad>().on)
			{
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
				playerRigidbody.AddForce(new Vector2(playerRigidbody.velocity.x, jumpad.GetComponent<JumpPad>().force));
			}	
		}	
		
		//Teleporter
		if(collidercheck.gameObject.tag=="Teleporter")
		{
			teleporterOverlap=true;
			interactObject = collidercheck.transform.parent.gameObject;
		}
	}
	
	void OnTriggerExit2D(Collider2D collidercheck)
	{
		if(collidercheck.gameObject.tag=="Teleporter")
			teleporterOverlap=false;
	}

	void OnCollisionEnter2D(Collision2D collider)
	{
		if (collider.gameObject.tag == "Enemy")
			StartCoroutine(EnemyKnockback(collider.gameObject.transform));
	}

	// 1: Controls
	void Controls()
	{
		// Console control.
		if(Input.GetKeyDown(consoleButton))
		{
			if(usingConsole)
			{
				consoleCamera.SetActive(false);
				playerCamera.SetActive(true);
				usingConsole = false;
			}
			else
			{
				playerCamera.SetActive(false);
				consoleCamera.SetActive(true);
				usingConsole = true;
			}
		}

		// Movement controls.
		if(!usingConsole)
		{
			//Keyboard
			if(Input.GetKey(right) || (Input.GetAxis("Horizontal") > 0 && playerID == 2))
			{
				movingRight=true;
				movingLeft=false;

				if(Physics2D.Raycast(feetCollider.bounds.center , Vector2.right, feetCollider.bounds.extents.x + 0.01f, physicalColliders))//!!! .1f value?
					collidingWall = 1;

				else
					collidingWall = 0;
			}
				
			else if(Input.GetKey(left) || (Input.GetAxis("Horizontal") < 0 && playerID == 2))
			{
				movingLeft=true;
				movingRight=false;

				if(Physics2D.Raycast(feetCollider.bounds.center , -Vector2.right, feetCollider.bounds.extents.x + 0.01f, physicalColliders))//!!! .1f value?
					collidingWall = -1;

				else
					collidingWall = 0;
			}
				
			else 
			{
				movingLeft=false;
				movingRight=false;

				collidingWall = 0;
			}
		}

		if(grounded && !usingConsole && (Input.GetKeyDown(jump)  || (Input.GetButtonDown("Jump") && playerID == 2)))
		{
			StartCoroutine(Jumping());
		}

		else if(!grounded && !usingConsole && (Input.GetKeyDown(jump)  || (Input.GetButtonDown("Jump") && playerID == 2)))
			jumpedTwice = true;

		if(!grounded && (Input.GetKey(jump)  || (Input.GetButton("Jump") && playerID == 2))  && jumpedTwice)
			boosting = true;

		else
			boosting = false;

		if(!grounded && Input.GetKeyDown(jump) && jumpedTwice)
		{
			powerMeterPhotonView.RPC("JetpackStart", PhotonTargets.AllBuffered, isPlayer1);
		}
		else if(!grounded && Input.GetKeyUp(jump) && jumpedTwice)
		{
			powerMeterPhotonView.RPC("JetpackStop", PhotonTargets.AllBuffered, isPlayer1);
		}
	}

	// 2: Movement
	void Movement()
	{
		if(!usingConsole && !knockback)
		{
			if(movingRight && collidingWall != 1)
			{
				playerRigidbody.velocity = new Vector2(moveSpeed, playerRigidbody.velocity.y);
				moving = true;
			}
			
			else if(movingLeft && collidingWall != -1)
			{
				playerRigidbody.velocity = new Vector2(-moveSpeed, playerRigidbody.velocity.y);
				moving = true;
			}
			
			else
			{
				playerRigidbody.velocity= new Vector2(0, playerRigidbody.velocity.y);
				moving = false;
			}
			
			
			if(boosting)
			{
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, moveSpeed);
				EnableParticles(true);
				//GetComponent<PlayerAudio>().Boost(true);
			}
			
			else
			{
				EnableParticles(false);

////				try
////				{
////					GetComponent<PhotonView>().RPC("EnableParticles", PhotonPlayer.Find(otherPlayerID), false);
////				}
////
////				catch(Exception e)
////				{
////					Debug.LogWarning(e);
////				}
////
////				//GetComponent<PlayerAudio>().Boost(false);
			}
			
			
			//!!! Prevent from bouncing off teleporter
			if(teleporterOverlap)
			{
				if(!jumping && grounded)
					playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
			}
			
			//Grounded Check
			if(Physics2D.Raycast(feetCollider.bounds.center , -Vector2.up, feetCollider.bounds.extents.y + 0.01f, physicalColliders))//!!! .1f value?
			{
				grounded = true;
				jumpedTwice = false;
			}
			
			else
				grounded = false;
			
			//Limit Y Velocity
			if(playerRigidbody.velocity.y > 20)
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 20);
		}
	
		else
		{
			moving = false;
			
			if(knockback)
			{
				if(enemyHitPosX < gameObject.transform.position.x )
					playerRigidbody.velocity = new Vector2(15, 2);
				
				else
					playerRigidbody.velocity = new Vector2(-15, 2);
			}
			
			else
			{
				playerRigidbody.velocity = new Vector2(0, 0);   
            }	
        }	
	}

	// 3: Jumping
	IEnumerator Jumping()
	{
		jumping = true;
		playerRigidbody.AddForce(new Vector2(playerRigidbody.velocity.x, jumpForce));

		yield return new WaitForSeconds(.05f);
		jumping = false;
	}

	// 4: Animation
	void Animation()
	{	
		//float hSpeed = playerRigidbody.velocity.x;
		
		anim.SetBool("Moving", moving);
		anim.SetBool("Grounded", grounded);
		anim.SetBool("Teleporting", teleporting);		
		
		if(movingRight && !facingRight)
			Flip();
			
		else if(movingLeft && facingRight)
			Flip();	
	}

	// 5: Flip
	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
    }
	
	// 6: Enable Particles
	[PunRPC]
	void EnableParticles(bool enabled)
	{
		transform.FindChild("Particle Emitter").GetComponent<ParticleSystem>().enableEmission = enabled;
	}

	// 7: Enemy Knockback
	IEnumerator EnemyKnockback(Transform enemyTrans)
	{
		enemyHitPosX = enemyTrans.position.x;
		
		knockback = true;
		gameObject.layer = 14;
		
		yield return new WaitForSeconds(.2f);
		
		knockback = false;
		
		yield return new WaitForSeconds(1.8f);
		
		gameObject.layer = 8;
    }
}