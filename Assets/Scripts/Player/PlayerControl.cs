using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour 
{
	public int playerID;

	//Input Keys
	public KeyCode left, right, jump, interact;

	//bools
	public bool grounded, moving, movingRight, movingLeft, jumping, boosting, interacting, teleporting;
	public bool consoleOverlap, teleporterOverlap, switchOverlap, overrideOverlap;
	bool facingRight = true;
	bool tutorialViewed = false;

	//Physics Values
	float moveSpeed = 10, jumpForce = 1000;
	public GameObject interactObject;

	public int jumpCount = 0;

	Rigidbody2D playerRigidbody;

	CircleCollider2D []colliders;
	CircleCollider2D feetCollider;

	public LayerMask physicalColliders;
	
	Animator anim;

	public int collidingWall;

	void Start()
	{
		anim = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody2D>();
		colliders = gameObject.GetComponents<CircleCollider2D>();
		feetCollider = colliders[1];
	}

	void Update()
	{
		MovementControls();
		InteractControl();
		Animation();
	}

	void FixedUpdate()
	{
		Movement();
	}

	// 1: Movement Controls
	void MovementControls()
	{
		if(!interacting)
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

		if(grounded && !interacting && (Input.GetKeyDown(jump)  || (Input.GetButtonDown("Jump") && playerID == 2)))
		{
			StartCoroutine(Jumping());
		}

		else if(!grounded && !interacting && (Input.GetKeyDown(jump)  || (Input.GetButtonDown("Jump") && playerID == 2)))
			jumpCount = 2;

		if(!grounded && (Input.GetKey(jump)  || (Input.GetButton("Jump") && playerID == 2))  && jumpCount == 2)
			boosting = true;

		else
			boosting = false;

	}

	// 2: Movement
	void Movement()
	{
		if(!interacting)
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
			
			
			if(boosting && GetComponent<PlayerStats>().power > 0)
			{
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, moveSpeed);
				GetComponent<PlayerStats>().DecreasePower(true);

				EnableParticles(true);

				//GetComponent<PlayerAudio>().Boost(true);
			}
			
			else
			{

				GetComponent<PlayerStats>().DecreasePower(false);

				EnableParticles(false);

//				try
//				{
//					GetComponent<PhotonView>().RPC("EnableParticles", PhotonPlayer.Find(otherPlayerID), false);
//				}
//
//				catch(Exception e)
//				{
//					Debug.LogWarning(e);
//				}
//
//				//GetComponent<PlayerAudio>().Boost(false);
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
				jumpCount = 0;
			}
			
			else
				grounded = false;
			
			//Limit Y Velocity
			if(playerRigidbody.velocity.y > 20)
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 20);
		}


		else
		{
			playerRigidbody.velocity = new Vector2(0, 0);
			moving = false;
		}	
	}

	// 3: Interact
	void InteractControl()
	{
		if(Input.GetKeyDown(interact))
		{
			//Console
			if(consoleOverlap)
			{
				if(interacting)
				{
					interacting = false;

					interactObject.GetComponent<ConsoleProperties>().DeactivateConsole();
					interactObject.GetComponent<ConsoleProperties>().endConsoleInteraction(gameObject);

//					try
//					{
//						interactObject.GetComponent<PhotonView>().RPC("DeactivateConsole", PhotonPlayer.Find(otherPlayerID));
//					}
//
//					catch(Exception e)
//					{
//						Debug.LogWarning(e);
//                  }
				}	
				
				else if(interactObject.GetComponent<ConsoleProperties>().occupied == false)
				{
					interacting = true;

					interactObject.GetComponent<ConsoleProperties>().ActivateConsole(gameObject);
					interactObject.GetComponent<ConsoleProperties>().startConsoleInteraction(gameObject);

//					try
//					{
//						interactObject.GetComponent<PhotonView>().RPC("ActivateConsole", PhotonPlayer.Find(otherPlayerID), null);
//					}
//
//					catch(Exception e)
//					{
//						Debug.LogWarning(e);
//					}

					GetComponent<AudioSource>().PlayDelayed(0f);
					if(!tutorialViewed)
					{
						tutorialViewed = true;
					}
				}
			}
		}
	}

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

	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
    }

	//TRIGGER
	void OnTriggerEnter2D(Collider2D collidercheck)
	{     
		//Console
		if(collidercheck.gameObject.tag=="Console")
		{
			consoleOverlap=true;
			interactObject = collidercheck.gameObject;
			interactObject.GetComponent<ConsoleProperties>().showHideButtonOverlay(true);
		} 

		//Override
		if(collidercheck.gameObject.tag=="Override")
		{
			overrideOverlap=true;
		}  
		
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
		if(collidercheck.gameObject.tag=="Console")
		{
			consoleOverlap=false;
			interactObject.GetComponent<ConsoleProperties>().showHideButtonOverlay(false);
		}

		//Override
		if(collidercheck.gameObject.tag=="Override")
		{
			overrideOverlap=false;
		} 
		
		if(collidercheck.gameObject.tag=="Teleporter")
			teleporterOverlap=false;
	}

	[PunRPC]
	void EnableParticles(bool enabled)
	{
		transform.FindChild("Particle Emitter").GetComponent<ParticleSystem>().enableEmission = enabled;
	}
}
