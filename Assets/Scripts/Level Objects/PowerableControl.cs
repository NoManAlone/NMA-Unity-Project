using UnityEngine;
using System;
using System.Collections;

public class PowerableControl : MonoBehaviour 
{	
	public LayerMask clickableLayermask;

	int otherPlayerID;

	PowerManager powerManager;

	void Start()
	{
		powerManager = GameObject.Find("Console").GetComponent<PowerManager>();
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{

			Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
			
			RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0, clickableLayermask);

			if(hit.collider != null)
			{
				Debug.Log("Clicked " + hit.collider.name);
				//If component is powered
				if(hit.transform.parent.GetComponent<Power>().powered)
				{
					powerManager.IncreaseAvailablePower(hit.transform.parent.GetComponent<Power>().powerCost);

					//Toggle Power state
					hit.transform.parent.GetComponent<Power>().TogglePowered(hit.transform.parent.tag);

//					try
//					{
//						hit.transform.parent.GetComponent<PhotonView>().RPC("TogglePowered", PhotonPlayer.Find(otherPlayerID), hit.transform.parent.tag);
//					}
//
//					catch(Exception e)
//					{
//						Debug.LogWarning(e);
//					}
				}

				//If component is unpowered
				else
				{
 				    if(hit.transform.parent.GetComponent<Power>().powerCost <= powerManager.availablePower)//if enough available. power turn on
					{
						powerManager.DecreaseAvailablePower(hit.transform.parent.GetComponent<Power>().powerCost);
						hit.transform.parent.GetComponent<Power>().TogglePowered(hit.transform.parent.tag);

//						try
//						{
//							hit.transform.parent.GetComponent<PhotonView>().RPC("TogglePowered", PhotonPlayer.Find(otherPlayerID), hit.transform.parent.tag);
//						}
//
//						catch(Exception e)
//						{
//							Debug.LogWarning(e);
//						}
					}
				}
			}
		}
	}
}
