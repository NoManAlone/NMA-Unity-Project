using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsoleProperties : MonoBehaviour 
{
	public Vector2 cameraPosition;
	public float cameraSize;

	public bool occupied;
	public GameObject occupant;
	GameObject consoleUI, normalViewUI;

	void Awake()
	{
		consoleUI = GameObject.Find("ConsoleUI");
		normalViewUI = GameObject.Find ("NormalViewUI");
	}

	[PunRPC]
	public void ActivateConsole(GameObject go)
	{
		occupied = true;

		if(go != null)
		{
			occupant = go;
			occupant.GetComponent<PowerableControl>().enabled = true;
		}			
	}

	[PunRPC]
	public void DeactivateConsole()
	{
		occupied = false;

		if(occupant != null)
		{
			occupant.GetComponent<PowerableControl>().enabled = false;
			occupant = null;
		}	
	}

	public void startConsoleInteraction(GameObject player)
	{
		player.transform.Find("Camera").GetComponent<CameraControl>().snapToConsoleView(cameraPosition, cameraSize, player);

		if (!GameManager.t2p) 
		{
			if (consoleUI != null)//***
				consoleUI.SetActive (true);
		}

		
//		if(normalViewUI == null)
//			normalViewUI.SetActive(false);
		showHideButtonOverlay(false);
	}

	public void endConsoleInteraction(GameObject player)
	{
		//Camera.main.GetComponent<CameraControl>().snapToPlayerView();
		player.transform.Find("Camera").GetComponent<CameraControl>().snapToPlayerView(player);

		if(consoleUI != null)//***
			consoleUI.SetActive(false);
		if(normalViewUI == null)
			normalViewUI.SetActive(true);
	}

	public void showHideButtonOverlay(bool showing)
	{
		if(showing&&!occupied)
			transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
		else
			transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
